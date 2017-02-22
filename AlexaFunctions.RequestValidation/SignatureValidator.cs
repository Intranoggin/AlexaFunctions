using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
//using Org.BouncyCastle.X509;
//using Org.BouncyCastle.Security.Certificates;
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
            log.Info("line VS-A");
            string certCacheKey = _getCertCacheKey(certChainUrl);
            log.Info("line VS-B");
            X509Certificate2 cert = MemoryCache.Default.Get(certCacheKey) as X509Certificate2;
            log.Info("line VS-C");
            if (cert == null ||
                !CheckRequestSignature(requestData, expectedSignature, cert,log))
            {
                log.Info("line VS-D");
                // download the cert 
                // if we don't have it in cache or
                // if we have it but it's stale because the current request was signed with a newer cert
                // (signaled by signature check fail with cached cert)

                log.Info($"certChainUrl={certChainUrl.ToString()}");
                cert = RetrieveAndVerifyCertificate(certChainUrl,log);
                log.Info($"cert={cert.ToString()}");
                log.Info("line VS-E");
                if (cert == null) return false;
                log.Info("line VS-F");
                MemoryCache.Default.Set(certCacheKey, cert, _policy);
                log.Info("line VS-G");
            }
            log.Info("line VS-H");
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
            log.Info("RVC 1");
            if (!VerifyCertificateUrl(certChainUrl)) return null;
            log.Info("RVC 2");
            using (var webClient = new WebClient())
            {
                string certContent = webClient.DownloadString(certChainUrl);
                log.Info($"RVC 3 Cert Content={certContent}");
                

                //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(certContent));
                var cert = new X509Certificate2(Encoding.UTF8.GetBytes(certContent));
                log.Info("RVC 4");
                var effectiveDate = DateTime.MinValue;
                var expiryDate = DateTime.MinValue;

                log.Info("RVC 5");
                if (!(
                DateTime.TryParse(cert.GetEffectiveDateString(), out effectiveDate) &&
                effectiveDate < DateTime.Now &&
                DateTime.TryParse(cert.GetExpirationDateString(), out expiryDate) &&
                expiryDate > DateTime.Now
                ))
                {

                    log.Info("RVC 6");
                    throw new Exception("Certificate Error");
                }

                log.Info("RVC 7");
                return cert;
            }

                //var cert = (X509Certificate)pemReader.ReadObject();
            //    try
            //    {
            //        cert.CheckValidity();
            //        if (!CheckCertSubjectNames(cert)) return null;
            //    }
            //    catch (CertificateExpiredException)
            //    {
            //        return null;
            //    }
            //    catch (CertificateNotYetValidException)
            //    {
            //        return null;
            //    }
            
            //return cert;
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


            // making requests to externally-supplied URLs is an open invitation to DoS
            // so restrict host to an Alexa controlled subdomain/path
            //if (!VerifyCertificateUrl(certChainUrl)) return null;

            //var httpClient = new HttpClient();
            //var httpResponse = await httpClient.GetAsync(certChainUrl);
            //var content = await httpResponse.Content.ReadAsStringAsync();
            //if (String.IsNullOrEmpty(content)) return null;

            //var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StringReader(content));
            //var cert = (X509Certificate)pemReader.ReadObject();
            //try
            //{
            //    cert.CheckValidity();
            //    if (!CheckCertSubjectNames(cert)) return null;
            //}
            //catch (CertificateExpiredException)
            //{
            //    return null;
            //}
            //catch (CertificateNotYetValidException)
            //{
            //    return null;
            //}

            //return cert;
        }



        public static bool CheckRequestSignature(
            string requestData, string expectedSignature, X509Certificate2 cert, TraceWriter log)
        {
            log.Info("line X-A");
            byte[] signature = null;
            try
            {
                log.Info("line X-B");
                signature = Convert.FromBase64String(expectedSignature);
                log.Info($"expectedSignature={expectedSignature}");
                log.Info($"expectedSig={signature.ToString()}");
                log.Info("line X-C");
            }
            catch (FormatException)
            {
                log.Info("line X-D");
                return false;
            }

            using (var sha1 = new SHA1Managed())
            {
                log.Info("line X-E");
                //var body = null;
                var data = sha1.ComputeHash(Encoding.UTF8.GetBytes(requestData));

                log.Info("line X-F");
                var rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

                log.Info("line X-G");
                return rsa.VerifyHash(data,CryptoConfig.MapNameToOID("SHA1"), signature);
            }
                log.Info("line X-H");
            return true;
            //var publicKey = (Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters)cert.GetPublicKey();
            //log.Info("line X-F");
            //var signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner(ControlConstants.SIGNATURE_ALGORITHM);
            //log.Info("line X-G");
            //signer.Init(false, publicKey);
            //log.Info("line X-H");
            //signer.BlockUpdate(serializedSpeechletRequest, 0, serializedSpeechletRequest.Length);
            //log.Info("line X-I");
            //return signer.VerifySignature(signature);
        }


    
        //private static bool CheckCertSubjectNames(X509Certificate2 cert)
        //{
        //    bool found = false;
            
        //    ArrayList subjectNamesList = (ArrayList)cert.GetSubjectAlternativeNames();
        //    for (int i = 0; i < subjectNamesList.Count; i++)
        //    {
        //        ArrayList subjectNames = (ArrayList)subjectNamesList[i];
        //        for (int j = 0; j < subjectNames.Count; j++)
        //        {
        //            if (subjectNames[j] is String && subjectNames[j].Equals(ControlConstants.ECHO_API_DOMAIN_NAME))
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //    }

        //    return found;
        //}
    }
}
