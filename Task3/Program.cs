using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Task3.Models;

namespace Task3
{
    // linq to xml
    // Проект складається з класів CarType, Record, Client та CarPark
    /*
     * Архітектура проекту наступа: автопарк містить в собі автомобілі, клієнтів та записи. Це звязок один до одного, проте якщо розширювати програму то можна додати поле CarParkId та зробити
     * це звязком один до багатьох. Автомобілі мають в собі тип CarType, і тут в нас тоже звязок один до багатьох оскільки багато автомобілів можуть мати один тип.
     * CarType - це клас який собою представляє тип автомобіля в автопарку
     * Record - клас який представляє запис автопарку, який містить номер клієнта та автомобіля, а також дату початку та кінця оренди, а також ціну та завдаток. Окрім цього є допоміжні дані для того
     * щоб визначити чи автомобіль повернуто, та чи повернуто вчасно
     * Client - клас який представляє клієнта, містить імя, адресу та номер телефону
     * CarPark - клас який представляє автопарк, він містить в собі колекцію автомобілів, записів та клієнтів.
     * окрім цього цей клас містить функції для видачі авто, та прийому авто, які в свою чергу містять всю обробку інформації та створюють або редагують
     * відповідні обєкти у внутрішніх колеціях. Також є функція реєстрації клієнта яка дозволяє зареєструвати клієнта коли він орендує авто.
     * В основному класі Program є ткі функції:
     * initializePark() - ініціалізація автопарку даними автоматично.
     * initializeFromConsole() - ініціалізація даних вручну з консолі. цей метод викликає методи enterRecords, enterClients, enterCar.
     * enterCar() - метод для введення автомобілів з консолі вручну. тут в нескінчекнному циклі користувача просять вводити дані поки він не вибере функцію завершити.
     * enterClients() - метод для введення клієнтів з консолі вручну. тут в нескінчекнному циклі користувача просять вводити дані поки він не вибере функцію завершити.
     * enterRecords() - метод для введення записів з консолі вручну. тут в нескінчекнному циклі користувача просять вводити дані поки він не вибере функцію завершити.
     * writeToXml() - в цьому методів за допомогою XmlWriter створюється новий документ, після цього він наповнюється даними з обєкту carPark за допомогою функцій WriteStartDocument, WriteStartElement та ін.
     * В Xml дані записуються у вигляді тегів одиничними, та містять атрибутами усю інформацію про себе.
     * readFromXml() - метод читання даних з Xml, він в собі викликає showCarsFromXml, showClientsFromXml, showRecordsFromXml.
     * showCarsFromXml() - читає дані з Xml файлу за допомогою XDocument, та витягує з елементу його атрибути за допомогою XAttribute
     * так само читаються записи та клієнти.
     * xmlQueries() - метод який містить в собі запити до документа з використанням LINQ to XML. Кожен запит містить коментар з коротким поясненням
     * тут є багато різних запитів з використанням join,select,where, orderBy з документом Xml
     *
     * До кожного запиту написаний коментар. Запити включають в себе такі види функції LINQ як групування, фільтрація, вибірка та групування, та інші.
     */

    internal class Program
    {
        private static List<CarType> carTypes;

        private static void Main(string[] args)
        {
            carTypes = new List<CarType>
           {
               new CarType{Id=1, Name="Crossroad"},
               new CarType{Id=2, Name="Combo"},
               new CarType{Id=3, Name="Leight"},
               new CarType{Id=4, Name="Heavy"},
           };
            CarPark carPark = new CarPark();
            //initializePark(carPark);
            initializeFromConsole(carPark);
            writeToXml(carPark);
            //readFromXml();
            xmlQueries();
        }

        private static void xmlQueries()
        {
            XDocument xmlDoc = XDocument.Load(@"D:\Task1\Task3\test.xml");
            // сортування авто за цінами
            var s1 = xmlDoc.Descendants("carPark").Descendants("cars").Descendants("car").OrderBy(c => int.Parse(c.Attribute("Price").Value));
            foreach (var s in s1)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("--------");
            // filter car by price
            var s2 = from c in xmlDoc.Element("carPark").Elements("cars").Elements("car")
                     where (int)c.Attribute("Price") < 200
                     select c;
            foreach (var s in s2)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("--------");
            // select all brands
            var s3 = xmlDoc.Element("carPark").Elements("cars").Elements("car").Select(c => new { Brand = c.Attribute("Brand").Value });
            foreach (var s in s3)
            {
                Console.WriteLine(s.Brand);
            }
            Console.WriteLine("--------");
            // join cars and record and select ID and price
            var s4 = from c in xmlDoc.Element("carPark").Elements("cars").Elements("car")
                     join r in xmlDoc.Element("carPark").Elements("records").Elements("record") on c.Attribute("Id").Value equals r.Attribute("CarId").Value
                     select new { carId = c.Attribute("Id").Value, recordId = r.Attribute("ClientId").Value, price = r.Attribute("Price").Value };
            foreach (var s in s4)
            {
                Console.WriteLine($"{s.carId} | {s.recordId} | {s.price}");
            }
            Console.WriteLine("--------");
            // complicated join client and records
            var s5 = xmlDoc.Element("carPark").Elements("clients").Elements("client")
                    .Join(
                          xmlDoc.Element("carPark").Elements("records").Elements("record"),
                          c => c.Attribute("Id").Value, r => r.Attribute("ClientId").Value,
                          (c1, r1) => new
                          {
                              clientName = c1.Attribute("Name").Value,
                              recordPrice = r1.Attribute("Price").Value,
                              recordCarId = r1.Attribute("CarId").Value,
                          }
                          );
            foreach (var s in s5)
            {
                Console.WriteLine($"{s.clientName} | {s.recordPrice} | {s.recordCarId}");
            }
            Console.WriteLine("--------");
            // select all client which number starts with 099
            var s6 = from c in xmlDoc.Element("carPark").Elements("clients").Elements("client")
                     where c.Attribute("PhoneNumber").Value.StartsWith("099")
                     select new { name = c.Attribute("Name").Value, phone = c.Attribute("PhoneNumber").Value };
            foreach (var s in s6)
            {
                Console.WriteLine($"{s.name} | {s.phone}");
            }
            Console.WriteLine("--------");
            // select count of records which is returned in time
            var s7 = xmlDoc.Element("carPark").Elements("records").Elements("record")
                .Where(c => c.Attribute("IsReturnedInTime").Value == "True").Count();
            Console.WriteLine("Returned in time " + s7 + " records");
            Console.WriteLine("--------");
            // 3-x join (very complicated)
            var s8 = from r in xmlDoc.Element("carPark").Elements("records").Elements("record")
                     join c1 in xmlDoc.Element("carPark").Elements("cars").Elements("car")
                     on r.Attribute("CarId").Value equals c1.Attribute("Id").Value
                     join c2 in xmlDoc.Element("carPark").Elements("clients").Elements("client")
                     on r.Attribute("ClientId").Value equals c2.Attribute("Id").Value
                     select new
                     {
                         carBrand = c1.Attribute("Brand").Value,
                         clientName = c2.Attribute("Name").Value,
                         price = r.Attribute("Price").Value
                     };
            foreach (var s in s8)
            {
                Console.WriteLine($"{s.clientName} | {s.carBrand} | {s.price}");
            }
            Console.WriteLine("--------");
            // select record with price from 8000 and from 10000, using TAKEWhile, SKIPWhile
            var s9 = xmlDoc.Element("carPark").Elements("records").Elements("record")
               .SkipWhile(r => int.Parse(r.Attribute("Price").Value) < 8000)
               .TakeWhile(r => int.Parse(r.Attribute("Price").Value) > 10000)
               .Select(r => new
               {
                   id = r.Attribute("Id").Value,
                   price = r.Attribute("Price").Value,
                   start = r.Attribute("StartDate").Value,
                   end = r.Attribute("EndDate").Value
               });
            foreach (var s in s9)
            {
                Console.WriteLine($"{s.id} | {s.price} | {s.start} | {s.end}");
            }
            Console.WriteLine("--------");
            //
            var s10 = from r in xmlDoc.Element("carPark").Elements("records").Elements("record")
                      join c1 in xmlDoc.Element("carPark").Elements("cars").Elements("car")
                      on r.Attribute("CarId").Value equals c1.Attribute("Id").Value
                      join c2 in xmlDoc.Element("carPark").Elements("clients").Elements("client")
                      on r.Attribute("ClientId").Value equals c2.Attribute("Id").Value
                      where int.Parse(c1.Attribute("Price").Value) > 3000
                      || int.Parse(r.Attribute("Price").Value) > 10000
                      orderby c1.Attribute("Brand").Value
                      select new
                      {
                          carBrand = c1.Attribute("Brand").Value,
                          clientName = c2.Attribute("Name").Value,
                          carPrice = c1.Attribute("Price").Value,
                          price = r.Attribute("Price").Value,
                          start = r.Attribute("StartDate").Value,
                          end = r.Attribute("EndDate").Value
                      };
            foreach (var s in s10)
            {
                Console.WriteLine($"{s.clientName} | {s.carBrand} | {s.carPrice} | {s.start} | {s.end} | {s.price}");
            }
            Console.WriteLine("--------");
        }

        private static void readFromXml()
        {
            showCarsFromXml();
            showClientsFromXml();
            showRecordsFromXml();
        }

        private static void showCarsFromXml()
        {
            XDocument xmlDoc = XDocument.Load(@"D:\Task1\Task3\test.xml");
            foreach (var element in xmlDoc.Element("carPark").Element("cars").Elements("car"))
            {
                XAttribute idAttr = element.Attribute("Id");
                XAttribute brandAttr = element.Attribute("Brand");
                XAttribute priceAttr = element.Attribute("Price");
                XAttribute typeAttr = element.Attribute("Type");
                Console.WriteLine($"Автомобiль №{idAttr.Value}\nМарка: {brandAttr.Value}\nЦiна: {priceAttr.Value}\nТип: {typeAttr.Value}\n-------------------");
            }
        }

        private static void showClientsFromXml()
        {
            XDocument xmlDoc = XDocument.Load(@"D:\Task1\Task3\test.xml");
            foreach (var element in xmlDoc.Element("carPark").Element("clients").Elements("client"))
            {
                XAttribute idAttr = element.Attribute("Id");
                XAttribute nameAttr = element.Attribute("Name");
                XAttribute addresAttr = element.Attribute("Address");
                XAttribute phoneAttr = element.Attribute("PhoneNumber");
                Console.WriteLine($"Клiєнт №{idAttr.Value}\niм'я: {nameAttr.Value}\nАдрес: {addresAttr.Value}\nНомер телефону: {phoneAttr.Value}\n-------------------");
            }
        }

        private static void showRecordsFromXml()
        {
            XDocument xmlDoc = XDocument.Load(@"D:\Task1\Task3\test.xml");
            foreach (var element in xmlDoc.Element("carPark").Element("records").Elements("record"))
            {
                XAttribute idAttr = element.Attribute("Id");
                XAttribute startDateAttr = element.Attribute("StartDate");
                XAttribute endDateAttr = element.Attribute("EndDate");
                XAttribute prePriceAttr = element.Attribute("PrePrice");
                XAttribute priceAttr = element.Attribute("Price");
                XAttribute carIdAttr = element.Attribute("CarId");
                XAttribute clientIdAttr = element.Attribute("ClientId");
                XAttribute isReturnedAttr = element.Attribute("IsReturned");
                XAttribute isReturnedInTimeAttr = element.Attribute("IsReturnedInTime");
                Console.WriteLine($"Запис №{idAttr.Value}\nДата видачi: {startDateAttr.Value}\nДата повернення: {endDateAttr.Value}\nЗавдаток {prePriceAttr.Value}" +
                    $"\nЦiна: {priceAttr.Value}\niдентифiкатор автомобiля: {carIdAttr.Value}\nдентифiкатор клiєнта {clientIdAttr.Value}" +
                    $"\nЧи повернено: {isReturnedAttr.Value}\nЧи повернено вчасно: {isReturnedInTimeAttr.Value}\n-------------------");
            }
        }

        private static void writeToXml(CarPark park)
        {
            XmlWriter xmlWriter = XmlWriter.Create(@"D:\Task1\Task3\test.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("carPark");
            xmlWriter.WriteStartElement("cars");
            foreach (var car in park.cars)
            {
                xmlWriter.WriteStartElement("car");
                xmlWriter.WriteAttributeString("Id", car.Id.ToString());
                xmlWriter.WriteAttributeString("Brand", car.Brand);
                xmlWriter.WriteAttributeString("Price", car.Price.ToString());
                xmlWriter.WriteAttributeString("Type", car.Type.Name);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("clients");
            foreach (var client in park.clients)
            {
                xmlWriter.WriteStartElement("client");
                xmlWriter.WriteAttributeString("Id", client.Id.ToString());
                xmlWriter.WriteAttributeString("Name", client.Name);
                xmlWriter.WriteAttributeString("Address", client.Address);
                xmlWriter.WriteAttributeString("PhoneNumber", client.PhoneNumber);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("records");
            foreach (var record in park.records)
            {
                xmlWriter.WriteStartElement("record");
                xmlWriter.WriteAttributeString("Id", record.Id.ToString());
                xmlWriter.WriteAttributeString("StartDate", record.StartDate.ToString("dd.MM.yyyy"));
                xmlWriter.WriteAttributeString("EndDate", record.EndDate.ToString("dd.MM.yyyy"));
                xmlWriter.WriteAttributeString("PrePrice", record.Pre_Price.ToString());
                xmlWriter.WriteAttributeString("Price", record.BillPrice.ToString());
                xmlWriter.WriteAttributeString("CarId", record.CarId.ToString());
                xmlWriter.WriteAttributeString("ClientId", record.ClientId.ToString());
                xmlWriter.WriteAttributeString("IsReturned", record.IsReturned.ToString());
                xmlWriter.WriteAttributeString("IsReturnedInTime", record.IsReturnedInTime.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private static void initializeFromConsole(CarPark park)
        {
            enterCar(park);
            enterClients(park);
            enterRecords(park);
        }

        private static void enterRecords(CarPark park)
        {
            bool state = true;
            int i = 0;
            while (state)
            {
                Console.WriteLine("Введiть iдентифiкацiйний номер клiєнта № " + (i + 1));
                var clientIdText = Console.ReadLine();
                var clientId = int.Parse(clientIdText);
                Console.WriteLine("Введiть iдентифiкацiйний номер автомобiля № " + (i + 1));
                var carIdText = Console.ReadLine();
                var carId = int.Parse(carIdText);

                Console.WriteLine("Введiть дату (формат dd.MM.YYYY) початку оренду № " + (i + 1));
                var startDateText = Console.ReadLine();
                var startDate = DateTime.Parse(startDateText);
                Console.WriteLine("Введiть дату (формат dd.MM.YYYY) кiнця оренди № " + (i + 1));
                var endDateText = Console.ReadLine();
                var endDate = DateTime.Parse(endDateText);
                Console.WriteLine("Введiть цiну завдатку № " + (i + 1));
                var prePriceText = Console.ReadLine();
                var prePrice = Double.Parse(prePriceText);
                Console.WriteLine("Введiть цiну повної оренди № " + (i + 1));
                var priceText = Console.ReadLine();
                var price = Double.Parse(priceText);

                park.giveCar(carId, clientId, startDate, endDate);
                Console.WriteLine("Припинити додавання записiв?\n[1] - Так\n[2] - Нi");
                int choice = int.Parse(Console.ReadLine());
                if (choice != 2)
                {
                    state = false;
                }
                i++;
            }
            Console.WriteLine("Вихiд");
        }

        private static void enterClients(CarPark park)
        {
            bool state = true;
            int i = 0;
            while (state)
            {
                Console.WriteLine("Введiть iм'я клiєнта № " + (i + 1));
                var name = Console.ReadLine();

                Console.WriteLine("Введiть адресу клiєнта № " + (i + 1));
                var addres = Console.ReadLine();

                Console.WriteLine("Виберiть номер телефону клiєнта № " + (i + 1));
                var phone = Console.ReadLine();

                park.registerClient(name, addres, phone);
                Console.WriteLine("Припинити додавання клiєнтiв?\n[1] - Так\n[2] - Нi");
                int choice = int.Parse(Console.ReadLine());
                if (choice != 2)
                {
                    state = false;
                }
                i++;
            }
            Console.WriteLine("Вихiд");
        }

        private static void enterCar(CarPark park)
        {
            bool state = true;
            int i = 0;
            while (state)
            {
                Console.WriteLine("Введiть марку автомобiля № " + (i + 1));
                var brand = Console.ReadLine();
                Console.WriteLine("Введiть цiну автомобiля № " + (i + 1));
                var priceText = Console.ReadLine();
                int price = 0;
                if (int.TryParse(priceText, out var res))
                {
                    price = res;
                }
                Console.WriteLine("Виберiть номер типу автомобiля № " + (i + 1) + "\n[1] Crossroad\n[2] Combo\n[3] Leight\n[4] Heavy");
                int id = int.Parse(Console.ReadLine());
                var type = carTypes.Find(r => r.Id == id);
                park.addCar(brand, price, type);
                Console.WriteLine("Припинити додавання автомобiлiв?\n[1] - Так\n[2] - Нi");
                int choice = int.Parse(Console.ReadLine());
                if (choice != 2)
                {
                    state = false;
                }
                i++;
            }
            Console.WriteLine("Вихiд");
        }

        private static void initializePark(CarPark carPark)
        {
            carPark.cars.AddRange(new List<Car>
            {
                new Car{Id=1, Brand="Toyota", Price = 100, Type = carTypes[0] },
                new Car{Id=2, Brand="Mercedes", Price = 120, Type = carTypes[1] },
                new Car{Id=3, Brand="Bugati", Price = 300, Type = carTypes[2] },
                new Car{Id=4, Brand="Lada", Price = 45, Type = carTypes[1] },
                new Car{Id=5, Brand="Kia", Price = 70, Type = carTypes[3] },
            });
            carPark.clients.AddRange(new List<Client>
            {
                new Client{ Id=1, Name="Ivan One", Address="Kyiv",       PhoneNumber="03939213811" },
                new Client{ Id=2, Name="Stepan Two", Address="Lviv",     PhoneNumber="09392123122" },
                new Client{ Id=3, Name="Ivan Three", Address="Odesa",    PhoneNumber="09912323233" },
                new Client{ Id=4, Name="Vasyl Four", Address="Zhytomyr", PhoneNumber="09811112333" },
                new Client{ Id=5, Name="Dima Five", Address="Kherson",   PhoneNumber="09612333323" },
            });
            carPark.giveCar(1, 1, DateTime.Parse("10.01.2020"), DateTime.Parse("20.01.2020"));
            carPark.giveCar(1, 1, DateTime.Parse("20.01.2020"), DateTime.Parse("25.01.2020"));
            carPark.giveCar(2, 2, DateTime.Parse("22.01.2020"), DateTime.Parse("30.01.2020"));
            carPark.giveCar(4, 5, DateTime.Parse("20.04.2020"), DateTime.Parse("27.04.2020"));
            carPark.giveCar(3, 4, DateTime.Parse("01.06.2020"), DateTime.Parse("03.06.2020"));
            carPark.giveCar(5, 4, DateTime.Parse("10.06.2020"), DateTime.Parse("20.06.2020"));
        }
    }
}