using System;
using System.Collections.Generic;
using System.Linq;
using Task2.Models;

namespace Task2
{
    // linq to objects
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
     * В основному класі Program є дві функції - перша це для ініціалізації автопарку даними, та друга це для обробки запитів над даними автопарку
     * До кожного запиту написаний коментар. Запити включають в себе такі види функції LINQ як групування, фільтрація, вибірка та групування, та інші.
     * УСІ ЗАПИТИ НАПИСАНІ З ВИКОРИСТАННЯМ LINQ-TO-OBJECTS.
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
            initializePark(carPark);
            linqQueries(carPark);
            //////////////////////////
        }

        private static void linqQueries(CarPark park)
        {
            Console.WriteLine($"---------------");

            // select all cars
            var query = from x in park.cars
                        select x;
            foreach (var x in query)
                Console.WriteLine($" {x.Id} - {x.Brand}");
            Console.WriteLine($"---------------");
            // select only brand of all cars
            var query2 = from x in park.cars
                         select x.Brand;
            foreach (var x in query2)
                Console.WriteLine($"{x}");
            Console.WriteLine($"---------------");
            // select only Combo  all cars
            var query3 = from x in park.cars
                         where x.Type.Name == "Combo"
                         select x;
            foreach (var x in query3)
                Console.WriteLine($" {x.Id} - {x.Brand}");
            Console.WriteLine($"---------------");
            // select only Combo or Heavy cars
            var query4 = from x in park.cars
                         where x.Type.Name == "Combo" || x.Type.Name == "Heavy"
                         select x;
            foreach (var x in query4)
                Console.WriteLine($" {x.Id} - {x.Brand}");
            Console.WriteLine($"---------------");
            // select new Anonymous type
            var query5 = from x in park.cars
                         select new { FirstValue = x.Id, secondValue = x.Brand };
            foreach (var x in query5)
                Console.WriteLine($" {x.FirstValue} - {x.secondValue}");
            Console.WriteLine($"---------------");
            // select cars which price < 300 and order by price and by brand
            var query6 = park.cars.Where(c => c.Price < 300).OrderBy(c => c.Price).ThenByDescending(c => c.Brand);
            foreach (var x in query6)
                Console.WriteLine($" {x.Id} - {x.Brand} - {x.Price}");
            Console.WriteLine($"---------------");
            // select car and records
            var query7 = from x in park.cars
                         join t in park.records on x.Id equals t.CarId
                         select new { id = x.Id, brand = x.Brand, price = t.BillPrice };
            foreach (var x in query7)
                Console.WriteLine($" {x.id} - {x.brand} - {x.price}");
            Console.WriteLine($"---------------");
            // take all from 100 to 300
            var q8 = park.cars.SkipWhile(p => p.Price < 100).TakeWhile(p => p.Price > 300);
            foreach (var x in q8)
                Console.WriteLine($" {x.Id} - {x.Brand} - {x.Price}");
            Console.WriteLine($"---------------");
            // decart multiple
            var q9 = from x in park.cars
                     from t in park.records
                     select new { carId = x.Id, recordId = t.Id };
            foreach (var x in q9)
                Console.WriteLine($" {x.carId} - {x.recordId}");
            Console.WriteLine($"---------------");
            // JOIN. take car and record from 200
            var q10 = from x in park.cars
                      join t in park.records on x.Id equals t.CarId
                      where t.BillPrice > 200
                      select new { Id = x.Id, Brand = x.Brand, Price = t.BillPrice };
            foreach (var x in q10)
                Console.WriteLine($" {x.Id} - {x.Brand} - {x.Price}");
            Console.WriteLine($"---------------");
            // select with join and group join
            var q11 = from x in park.cars
                      join t in park.records on x.Id equals t.CarId into temp
                      from i in temp
                      select new { v1 = x.Id, v2 = x.Brand, v3 = i.StartDate };
            foreach (var x in q11)
                Console.WriteLine($" {x.v1} - {x.v2} - {x.v3}");
            Console.WriteLine($"---------------");
            // select count of cars which bill is ended
            var q12 = (from x in park.cars
                       join t in park.records on x.Id equals t.CarId
                       where t.EndDate < DateTime.Now
                       select x).Count();
            Console.WriteLine($" {q12} ");
            Console.WriteLine($"---------------");
            // select count of cars which brand name start with T
            var q13 = park.cars.Where(c => c.Brand.StartsWith('T')).Count();
            Console.WriteLine($" {q13} ");
            Console.WriteLine($"---------------");
            // JOIN clients and records , and get start/end date and client name
            var q14 = park.clients.Join(park.records, p => p.Id, t => t.ClientId, (t1, t2) => new
            {
                startDate = t2.StartDate,
                endDate = t2.EndDate,
                clientName = t1.Name
            });
            foreach (var x in q14)
                Console.WriteLine($" {x.startDate} - {x.endDate} - {x.clientName}");
            Console.WriteLine($"---------------");
            // JOIN and NEW Anonymous type and TAKE operation
            var q15 = park.records.Join(park.cars, c => c.CarId, p => p.Id, (t1, t2) => new
            {
                billPrice = t1.BillPrice,
                prePrice = t1.Pre_Price,
                brand = t2.Brand,
                price = t2.Price
            }).Take(2);
            foreach (var x in q15)
                Console.WriteLine($" {x.billPrice} - {x.prePrice} - {x.brand} - {x.price}");
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