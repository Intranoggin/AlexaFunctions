using System;
using System.Net;
using System.Text;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.WebJobs.Host;

namespace AlexaFunctions.RequestValidation
{

    public class SignatureValidator
    {
        private static Func<string, string> _getCertCacheKey = (string url) => string.Format("{0}_{1}", ControlConstants.SIGNATURE_CERT_URL_REQUEST_HEADER, url);

        private static CacheItemPolicy _policy = new CacheItemPolicy
        {
            Priority = CacheItemPriority.Default,
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(24)
        };


        /// <summary>
        /// Verifying the Signature Certificate URL per requirements documented at
        /// https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/docs/developing-an-alexa-skill-as-a-web-service
        /// </summary>
        public static bool VerifyCertificateUrl(string certChainUrl)
        {
            if (String.IsNullOrEmpty(certChainUrl))
            {
                return false;
            }

            Uri certChainUri;
            if (!Uri.TryCreate(certChainUrl, UriKind.Absolute, out certChainUri))
            {
                return false;
            }

            return
                certChainUri.Host.Equals(ControlConstants.SIGNATURE_CERT_URL_HOST, StringComparison.OrdinalIgnoreCase) &&
                certChainUri.PathAndQuery.StartsWith(ControlConstants.SIGNATURE_CERT_URL_PATH) &&
                certChainUri.Scheme == Uri.UriSchemeHttps &&
                certChainUri.Port == 443;
        }


        /// <summary>
        /// Verifies request signature and manages the caching of the signature certificate
        /// </summary>
        public static bool VerifyRequestSignature(
            string requestData, string expectedSignature, string certChainUrl, TraceWriter log)
        {
            string certCacheKey = _getCertCacheKey(certChainUrl);
            X509Certificate2 cert = MemoryCache.Default.Get(certCacheKey) as X509Certificate2;
            if (cert == null ||
                !CheckRequestSignature(requestData, expectedSignature, cert,log))
            {
                // download the cert 
                // if we don't have it in cache or
                // if we have it but it's stale because the current request was signed with a newer cert
                // (signaled by signature check fail with cached cert)

                log.Info($"certChainUrl={certChainUrl.ToString()}");
                cert = RetrieveAndVerifyCertificate(certChainUrl,log);
                log.Info($"cert={cert.ToString()}");
                if (cert == null) return false;
                MemoryCache.Default.Set(certCacheKey, cert, _policy);
            }
            return CheckRequestSignature(requestData, expectedSignature, cert,log);
        }


        /// <summary>
        /// Verifies request signature and manages the caching of the signature certificate
        /// </summary>
        public async static Task<bool> VerifyRequestSignatureAsync(
            string requestData, string expectedSignature, string certChainUrl, TraceWriter log)
        {

            string certCacheKey = _getCertCacheKey(certChainUrl);
            X509Certificate2 cert = MemoryCache.Default.Get(certCacheKey) as X509Certificate2;
            if (cert == null ||
                !CheckRequestSignature(requestData, expectedSignature, cert,log))
            {

                // download the cert 
                // if we don't have it in cache or 
                // if we have it but it's stale because the current request was signed with a newer cert
                // (signaled by signature check fail with cached cert)
                cert = await RetrieveAndVerifyCertificateAsync(certChainUrl);
                if (cert == null) return false;

                MemoryCache.Default.Set(certCacheKey, cert, _policy);
            }

            return CheckRequestSignature(requestData, expectedSignature, cert,log);
        }


        public static X509Certificate2 RetrieveAndVerifyCertificate(string certChainUrl, TraceWriter log)
        {
            // making requests to externally-supplied URLs is an open invitation to DoS
            // so restrict host to an Alexa controlled subdomain/path
            if (!VerifyCertificateUrl(certChainUrl)) return null;
            using (var webClient = new WebClient())
            {
                string certContent = webClient.DownloadString(certChainUrl);
                        //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(certContent));
                var cert = new X509Certificate2(Encoding.UTF8.GetBytes(certContent));
                var effectiveDate = DateTime.MinValue;
                var expiryDate = DateTime.MinValue;

                if (!(
                DateTime.TryParse(cert.GetEffectiveDateString(), out effectiveDate) &&
                effectiveDate < DateTime.Now &&
                DateTime.TryParse(cert.GetExpirationDateString(), out expiryDate) &&
                expiryDate > DateTime.Now
                ))
                {

                    throw new Exception("Certificate Error");
                }

                return cert;
            }
        }


        public async static Task<X509Certificate2> RetrieveAndVerifyCertificateAsync(string certChainUrl)
        {
            // making requests to externally-supplied URLs is an open invitation to DoS
            // so restrict host to an Alexa controlled subdomain/path
            if (!VerifyCertificateUrl(certChainUrl)) return null;

            using (var webClient = new WebClient())
            {
                var certContent = await webClient.DownloadStringTaskAsync(certChainUrl);
                //webClient.downl
                //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(certContent));
                var cert = new X509Certificate2(certContent);
                var effectiveDate = DateTime.MinValue;
                var expiryDate = DateTime.MinValue;

                if (!(
                DateTime.TryParse(cert.GetEffectiveDateString(), out effectiveDate) &&
                effectiveDate < DateTime.Now &&
                DateTime.TryParse(cert.GetExpirationDateString(), out expiryDate) &&
                expiryDate > DateTime.Now
                ))
                {
                    throw new Exception("Certificate Error");
                }

                return cert;
            }
        }



        public static bool CheckRequestSignature(
            string requestData, string expectedSignature, X509Certificate2 cert, TraceWriter log)
        {
            byte[] signature = null;
            try
            {
                signature = Convert.FromBase64String(expectedSignature);
                log.Info($"expectedSignature={expectedSignature}");
                log.Info($"expectedSig={signature.ToString()}");
            }
            catch (FormatException)
            {
                return false;
            }

            using (var sha1 = new SHA1Managed())
            {
                //var body = null;
                var data = sha1.ComputeHash(Encoding.UTF8.GetBytes(requestData));

                var rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

                return rsa.VerifyHash(data,CryptoConfig.MapNameToOID("SHA1"), signature);
            }
        }
    }
}
