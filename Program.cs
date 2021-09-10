using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf.security;

namespace SignPDFusb
{

    class CertSelect
    {
        static void Main()
        {
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Test Certificate Select", "Select a certificate from the following list to get information on that certificate", X509SelectionFlag.MultiSelection);
            Console.WriteLine("Number of certificates: {0}{1}", scollection.Count, Environment.NewLine);

            foreach (X509Certificate2 x509 in scollection)
            {
                try
                {
                    byte[] rawdata = x509.RawData;
                    Console.WriteLine("Content Type: {0}{1}", X509Certificate2.GetCertContentType(rawdata), Environment.NewLine);
                    Console.WriteLine("Friendly Name: {0}{1}", x509.FriendlyName, Environment.NewLine);
                    Console.WriteLine("Certificate Verified?: {0}{1}", x509.Verify(), Environment.NewLine);
                    Console.WriteLine("Simple Name: {0}{1}", x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine);
                    Console.WriteLine("Signature Algorithm: {0}{1}", x509.SignatureAlgorithm.FriendlyName, Environment.NewLine);
                    Console.WriteLine("Private Key: {0}{1}", x509.PrivateKey.ToXmlString(false), Environment.NewLine);
                    Console.WriteLine("Certificate Archived?: {0}{1}", x509.Archived, Environment.NewLine);
                    Console.WriteLine("Length of Raw Data: {0}{1}", x509.RawData.Length, Environment.NewLine);
                    X509Certificate2UI.DisplayCertificate(x509);
                    x509.Reset();
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Information could not be written out for this certificate.");
                }
                
                string filename = @"C:\Users\j_far\source\repos\sitm\PRUEBA.pdf";
                PdfReader inputPdf = new PdfReader(filename);
                FileStream signedPdf = new FileStream(@"C:\Users\j_far\source\repos\sitm\PRUEBA_signed.pdf", FileMode.Create);
                PdfStamper pdfStamper = PdfStamper.CreateSignature(inputPdf,signedPdf,'\0');
                IExternalSignature externalSignature = new X509Certificate2Signature(x509, "SHA-256");
                PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                signatureAppearance.Reason = "My Signature";
                signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(0, 00, 200, 100), inputPdf.NumberOfPages, "Signature");
                signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
                MakeSignature.SignDetached(signatureAppearance, externalSignature, null, null, null, null, 0, CryptoStandard.CMS);
                inputPdf.Close();
                pdfStamper.Close();

            }
            store.Close();
            /* 
             
              
             }*/
        }

    }
}

            
         





     
