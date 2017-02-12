using Microsoft.Owin.Hosting;
using System;
using System.IO;
using System.Collections;
using System.Xml.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace OWINRestService 
{
    public class Program
    {
        private MunicipalityTaxes MunicipalityTaxes = new MunicipalityTaxes();

        static void Main()
        {

            string baseAddress = "http://127.0.0.1:8080/";

            // Start OWIN host 
            using (WebApp.Start(url: baseAddress))
            {
                Console.WriteLine("Service Listening at " + baseAddress);

                Program t = new Program();
                Console.WriteLine("Database Consist of:");
                t.XMLReader("taxes.xml");

                Console.WriteLine("Press A to add a Record");
                Console.WriteLine("Press S to Search by Municipality Name and Date");
                Console.WriteLine("Press I to Import from file");
                Console.WriteLine("Press E to Exit");

                while (true)
                {
                    var input = Console.ReadKey();

                    switch (input.Key)
                    {
                        case ConsoleKey.A:
                                  t.caseAdd();
                            break;
                        case ConsoleKey.S:
                                    t.caseSearch();
                            break;
                        case ConsoleKey.I:
                            Console.WriteLine("Please provide a file name. E.g.: 'import.xml'");
                                    t.caseImport();
                            break;
                        case ConsoleKey.E:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                }
            }
        }
                private void caseAdd()
        {
            Console.WriteLine("Please specify the municipality");
            var muni1 = Console.ReadLine(); //Error check

            Console.WriteLine("Please specify the date, format: yyyy-mm-dd");
            var date1 = Console.ReadLine();  //Error check                 
            XMLWriter(muni1, Convert.ToDateTime(date1), scheduleTax(DateTime.Parse(date1)));
            Console.WriteLine("row added");
        }

        private void caseSearch()
        {
            Console.WriteLine("Please specify the municipality");
            var muni = Console.ReadLine();//Error check 
            Console.WriteLine("Please specify the date, format: yyyy-mm-dd");

            string line1 = Console.ReadLine();//Error check 
            DateTime dt;
                if (!DateTime.TryParseExact(line1, "yyyy-mm-dd", null, System.Globalization.DateTimeStyles.None, out dt))
                {
                    Console.WriteLine("Invalid date, please retry");
                }

            SearchTax(muni, dt);
        }

        private void caseImport()
        {
            try
            {
                string filename = Console.ReadLine();
                XMLReader(filename);
                XMLimport(filename);
            }
            catch (IOException)
            {
                Console.WriteLine("File not found, make sure you are typing in: 'import.xml'");
                caseImport();
            }
        }
        private float scheduleTax(DateTime date)
        {

            System.DateTime yearTaxTo = new System.DateTime(2016, 12, 31);
            System.DateTime monthTaxTo = new System.DateTime(2016, 5, 31);

            //daily tax scheduled = 0.1 (at days 2016.01.01 and 2016.12.25).
            if (DateTime.Equals(DateTime.Parse("2016-01-01"), date) || DateTime.Equals(DateTime.Parse("2016-12-25"), date))
            {
                return (float)0.1;
            }
            //monthly tax = 0.4 (for period 2016.05.01-2016.05.31),
            else if (monthTaxTo.Subtract(date).Days < 31 && monthTaxTo.Subtract(date).Days > 0)
            {
                return (float)0.4;
            }//yearly tax = 0.2 (for period 2016.01.01-2016.12.31)
            else if (yearTaxTo.Subtract(date).Days < 365 && yearTaxTo.Subtract(date).Days > 0)
            {
                System.TimeSpan d = date.Subtract(date);
                return (float)0.2;
            }
            else
            {
                Console.WriteLine("Date is outside the scheduled taxes");
                return 0;
            }
        }

        private void SearchTax(string searchMunicipalityName, DateTime searchTaxDate)
        {
            DateTime testDate = new DateTime(searchTaxDate.Year, searchTaxDate.Month, searchTaxDate.Day, 0,0,0);

            XElement root = XElement.Load("taxes.xml");
            IEnumerable<XElement> tests =
                from el in root.Elements("Municipality")
                where (string)el.Element("MunicipalityName") == searchMunicipalityName && (DateTime)el.Element("TaxDate") == testDate
                select el;
            foreach (XElement el in tests)
                Console.WriteLine((string)el.Element("Tax").Value);
        }


        private void XMLWriter(string municipalityName, DateTime date, float tax)
        {
            MunicipalityTaxes.CollectionName = "Taxes";
            Municipality municipalitySelected = new Municipality(municipalityName, date, tax);
            MunicipalityTaxes.Add(municipalitySelected);
            XmlSerializer x = new XmlSerializer(typeof(MunicipalityTaxes));
            TextWriter writer = new StreamWriter("taxes.xml");
            x.Serialize(writer, MunicipalityTaxes);
            writer.Close();

        }

        private void XMLReader(string filename)
        {

            StringBuilder result = new StringBuilder();
            var n = XElement.Load(filename).Elements("Municipality");
            foreach (XElement level1Element in n)
            {
                string name = (level1Element.Element("MunicipalityName").Value);
                DateTime date = DateTime.Parse(level1Element.Element("TaxDate").Value);
                float tax = float.Parse(level1Element.Element("Tax").Value);

                result.AppendLine(level1Element.Element("MunicipalityName").Value);
                result.AppendLine(level1Element.Element("TaxDate").Value);
                result.AppendLine(level1Element.Element("Tax").Value);

                Municipality municipality = new Municipality(name, date, tax);
                MunicipalityTaxes.Add(municipality);
            }
            Console.WriteLine(result.ToString());
        }

        private void XMLimport(string filename)
        {
            XmlSerializer x = new XmlSerializer(typeof(MunicipalityTaxes));
            TextWriter writer = new StreamWriter("taxes.xml");
            x.Serialize(writer, MunicipalityTaxes);
            writer.Close();
        }
    }
}
public class MunicipalityTaxes : ICollection
{
    public string CollectionName;
    private ArrayList mArray = new ArrayList();

    public Municipality this[int index]
    {
        get { return (Municipality)mArray[index]; }
    }

    public void CopyTo(Array a, int index)
    {
        mArray.CopyTo(a, index);
    }
    public int Count
    {
        get { return mArray.Count; }
    }
    public object SyncRoot
    {
        get { return this; }
    }
    public bool IsSynchronized
    {
        get { return false; }
    }
    public IEnumerator GetEnumerator()
    {
        return mArray.GetEnumerator();
    }

    public void Add(Municipality newMTax)
    {
        mArray.Add(newMTax);
    }
}

public class Municipality
{
    public string MunicipalityName;
    public DateTime TaxDate;
    public float Tax;
    public Municipality() { }
    public Municipality(string mName, DateTime tDate, float tax)
    {
        MunicipalityName = mName;
        TaxDate = tDate;
        Tax = tax;
    }
}
            
        
    
