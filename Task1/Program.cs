using System;
using System.Data;
using System.Data.SqlClient;

namespace Task1
{
    internal class Program
    {
        // Для роботи з базою даних створено клас BaseConnection і в ньому всі методи для роботи. Також в папці Models є всі класи таблиць з бази даних.
        private static void Main(string[] args)
        {
            // тут потрібен  шлях до бази даних
            string connectionStr = "Data Source=DESKTOP-T829926;Initial Catalog=RieltorBase;Integrated Security=True";
            BaseConnection connection = new BaseConnection(connectionStr);

            // команда прочитати усі таблиці
            connection.selectAll();

            // команди Update, Create потребують передачі в них обєкт відповідної таблиці. Нижче наведений приклад оновлення і створення

            //connection.addBrockers(new Brockers() { Id = 6, FirstName = "User 5", LastName = "LN User1", PhoneNumber = "1111111111", AgencyId = 1 });
            //connection.updateBrockers(new Brockers() { Id = 6, FirstName = "User 5555", LastName = "LN User1", PhoneNumber = "1111111111", AgencyId = 1 });
            //connection.delete(5, "brockers");

            //connection.updateRoles(new Roles { Id = 1, Name = "Client" });

            // 10 запитів які потрібно було написати і обробити
            connection.tenQueries();

            Console.ReadLine();
        }
    }
}