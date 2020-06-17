using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Task3.Models
{
    internal class CarPark
    {
        public int Id { get; set; }
        public List<Car> cars;
        public List<Client> clients;
        public List<Record> records;

        public CarPark()
        {
            cars = new List<Car>();
            clients = new List<Client>();
            records = new List<Record>();
        }

        public void addCar(string brand, int price, CarType type)
        {
            if (this.cars.Count == 0)
            {
                this.cars.Add(new Car { Id = 1, Brand = brand, Price = price, Type = type });
            }
            else
            {
                this.cars.Add(new Car { Id = this.cars.Last().Id + 1, Brand = brand, Price = price, Type = type });
            }
            Console.WriteLine("Авто успiшно додано!");
        }

        public void giveCar(int carId, int clientId, DateTime startDate, DateTime endDate)
        {
            var car = this.cars.Find(r => r.Id == carId);
            if (car == null)
            {
                Console.WriteLine("Цього автомобiля немає в автопарку");
                return;
            }
            var client = this.clients.Find(r => r.Id == clientId);
            if (client == null)
            {
                Console.WriteLine("Цей клiєнт не зареєстрований\nПройдiть будь ласка реєстрацiю");
                return;
            }
            double price = (endDate.Subtract(startDate).TotalHours) * car.Price;
            if (this.records.Count == 0)
            {
                this.records.Add(new Record { Id = 1, BillPrice = price, CarId = car.Id, IsReturned = false, IsReturnedInTime = false, ClientId = client.Id, StartDate = startDate, EndDate = endDate, Pre_Price = price / 2 });
            }
            else
            {
                this.records.Add(new Record { Id = this.records.Last().Id + 1, CarId = car.Id, BillPrice = price, IsReturned = false, IsReturnedInTime = false, ClientId = client.Id, StartDate = startDate, EndDate = endDate, Pre_Price = price / 2 });
            }
            Console.WriteLine("Успiшно додано");
            return;
        }

        public void registerClient(string name, string address, string phoneNumber)
        {
            if (this.clients.Count == 0)
            {
                this.clients.Add(new Client { Id = 1, Address = address, Name = name, PhoneNumber = phoneNumber });
            }
            else
            {
                this.clients.Add(new Client { Id = this.clients.Last().Id + 1, Address = address, Name = name, PhoneNumber = phoneNumber });
            }
            Console.WriteLine("Клiєнта успiшно зареєстровано!");
        }

        public void returnCar(int carId, int cliendId, int recordId)
        {
            var record = this.records.Find(r => r.Id == recordId);
            var client = this.clients.Find(r => r.Id == cliendId);
            if (client == null || record == null)
            {
                Console.WriteLine("Такого клiєнту або запису не iснує! Спробуйте ще раз");
                return;
            }
            var car = this.cars.Find(c => c.Id == carId);
            if (car == null)
            {
                Console.WriteLine("Такого автомобiля не iснує. Спробуйте ще раз");
                return;
            }
            if (record.EndDate > DateTime.Now)
            {
                record.IsReturnedInTime = true;
                record.IsReturned = true;
            }
            else
            {
                record.IsReturnedInTime = false;
                record.IsReturned = true;
            }
            return;
        }
    }
}