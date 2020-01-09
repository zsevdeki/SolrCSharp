using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.SolrNetIntegration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using SolrCSharp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;


namespace SolrCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Startup.Init<Course>("http://localhost:8983/solr/MercDemo");
            //ISolrOperations<Media> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Media>>();

            IWindsorContainer container = new WindsorContainer();
            SolrNetFacility solrNetFacility = new SolrNetFacility("http://localhost:8983/solr");
            solrNetFacility.AddCore("core1", typeof(Media), "http://localhost:8983/solr/Document");
            solrNetFacility.AddCore("core2", typeof(Media2), "http://localhost:8983/solr/Metadata");
            container.AddFacility("solr", solrNetFacility);





            ISolrOperations<Media> solrCore1 = container.Resolve<ISolrOperations<Media>>("core1");
            ISolrOperations<Media2> solrCore2 = container.Resolve<ISolrOperations<Media2>>("core2");

            Console.WriteLine("Please enter query:");
            string queryFromUser = Console.ReadLine();

            while (!string.IsNullOrWhiteSpace(queryFromUser))
            {
                


                SolrQueryResults<Media> Medias = solrCore1.Query(queryFromUser);

                Console.WriteLine();
                Console.WriteLine("*Results*");
                Console.WriteLine("Total found: " + Medias.NumFound);

                int i = 0;
                foreach (Media media in Medias)
                {
                    Console.WriteLine(i++ + ": " + media.MediaId +" ");
                }

                Console.WriteLine();
                
                Console.Write("Execution time (ms): " + Medias.Header.QTime);

                SolrQueryResults<Media2> Medias2 = solrCore2.Query(queryFromUser);
                Console.WriteLine();
                Console.WriteLine("*Results*");
                Console.WriteLine("Total found: " + Medias2.NumFound);

                int j = 0;
                foreach (Media2 media2 in Medias2)
                {
                    Console.WriteLine(j++ + ": " + media2.MediaId + " ");
                }

                Console.WriteLine();

                Console.Write("Execution time (ms): " + Medias2.Header.QTime);

                //Saving Search Results

                Guid guid = Guid.NewGuid();

                string remoteUri = "http://localhost:8983/solr/Document/select?q=" + queryFromUser + "&wt=json&indent=true";
                string fileName = "SearchResults-"+guid+"-1.json", myStringWebResource = null;
                
                WebClient myWebClient = new WebClient();
             
                myStringWebResource = remoteUri + fileName;
               
               
                myWebClient.DownloadFile(myStringWebResource, fileName);
                Console.WriteLine("\nSuccessfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
                Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t");


                string remoteUri2 = "http://localhost:8983/solr/Metadata/select?q=" + queryFromUser + "&wt=json&indent=true";
                string fileName2 = "SearchResults-" + guid + "-2.json", myStringWebResource2 = null;
                WebClient myWebClient2 = new WebClient();
                myStringWebResource2 = remoteUri2 + fileName2;
                myWebClient2.DownloadFile(myStringWebResource2, fileName2);
                Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName2, myStringWebResource2);

                Console.ReadLine();




                Console.WriteLine(Environment.NewLine + "Please enter query:");
                queryFromUser = Console.ReadLine();

            }

            Console.WriteLine(Environment.NewLine + "Demo completed!");

        }
    }
}

