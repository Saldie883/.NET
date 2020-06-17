using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Task1
{
    /*
     * В цьому файлі зібрана уся логіка для роботи з базою даних.
     * Перш за все, визначено обєкт connectionString для того щоб утримувати рядок підключення до бази.
     * Далі по методах:
     *  update[Назва сутності] - це метод для оновлення певного запису у базі даних, він містить в  собі
     *  команду (SQL-скрипт) для оновлення запису в базі. Параметром приймає обєкт цієї таблиці(оновлений). такий метод прописаний для кожної сутності і це дозволяє нам
     *  оновити любий запис у любій табличці, просто передавши оновлений запис у метод
     *  delete(int id, string tableName) - видалити запис з таким ID у таблиці з назвою яку ми передамо, містить скрипт для видалення запису  з бази
     *  deleteAll(string tableName) - метод для видалення всіх записів  з певної таблиці
     *  add[назва таблиці] - метод для додавання запису у таблицю. В ньому є рядок (sql-скрипт) для вставлення нового запису, для цього потрібно передати параметром у цей метод
     *  новий (оновлений обєкт)
     *  select[назва таблиці] - отримати всі дані з певної таблиці та вивести їх на консоль. Ці методи викликають метод selectFromTable() з назвою таблицы
     *  selectFromTable(string tableName) - дістає усі дані з таблиці використовуючи SqlConnection SqlDataAdapter та DataSet, і потім у циклі перебираючи колонки та рядки виводяться дані на екран
     *  selectAll() - метод який викликає по черзі усі методи отримання даних з таблиць для того щоб вивести всю базу даних на консоль
     *  executeQuery(string query) - метод який створює зєднання з базою за допомогою SqlConnection та виконує запис який ми передали методом ExecuteNonQuery з обєкту SqlCommand
     * tenQueries() - це метод який виконує 10 запитів до бази даних згідно з поставленим завданням. Запити написані вручну та до кожного запиту є коментар з інформацією
     * загалом - запити містять в собі просту вибірку, фільтрацію,групування, та обєднання ( join)
     *

         */

    internal class BaseConnection
    {
        public string connectionString { get; set; }

        public BaseConnection(string connectionStr)
        {
            connectionString = connectionStr;
        }

        public void selectAll()
        {
            selectAllOperations();
            selectAllBrockers();
            selectAllAgency();
            selectAllInvoices();
            selectAllPersons();
            selectAllRealStates();
            selectAllRealStateTypes();
            selectAllBrockersRoles();
        }

        public void tenQueries()
        {
            List<string> queries = new List<string>();
            // take all brockers for specific agency
            queries.Add("select * from agencyes join brockers on brockers.agencyid=agencyes.id where agencyes.Name ='RoomEasy';");
            // take date, operation and brocker name from invoices
            queries.Add("select invoices.date,operations.name,brockers.firstname, brockers.lastname from invoices " +
                "join operations on operations.id=invoices.operationId " +
                "join brockers on brockers.id=invoices.brockerid; ;");
            // take clients,  date, operation and brocker name from invoices
            queries.Add("select invoices.date,operations.name,brockers.firstname, brockers.lastname, persons.name as Client from invoices " +
                "join operations on operations.id=invoices.operationId join brockers on brockers.id=invoices.brockerid" +
                " join persons on persons.id=invoices.clientId ;");
            // take all houses
            queries.Add("select * from realstates join realstatetypes on realstatetypes.id=realstates.typeid where realstatetypes.name='House' ;");
            // take all places
            queries.Add("select * from realstates join realstatetypes on realstatetypes.id=realstates.typeid where realstatetypes.name='Place' ;");
            // take all directors in database
            queries.Add("select * from persons join roles on roles.id=persons.roleid where roles.name='Director' ;");
            // take all agencies from kyiv
            queries.Add("select * from agencyes where address = 'Kyiv';");
            // take all agencies who have license
            queries.Add("select * from agencyes where license = 1;");
            // select all brockers who work in agency with license
            queries.Add("select * from brockers join agencyes on agencyes.id=brockers.agencyid where agencyes.license=1;");
            // select all invoices which brockers work in agency with license
            queries.Add("select invoices.id,invoices.date,brockers.firstname,brockers.lastname, agencyes.name  from invoices " +
                "join brockers on invoices.brockerid=brockers.id " +
                "join agencyes on agencyes.id=brockers.agencyid where agencyes.license=1;");
            foreach (var item in queries)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(item, connection);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    foreach (DataTable dt in ds.Tables)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Query: " + queries.IndexOf(item));
                        Console.WriteLine();
                        foreach (DataRow row in dt.Rows)
                        {
                            var cells = row.ItemArray;
                            foreach (object cell in cells)
                                Console.Write("\t{0}", cell);
                            Console.WriteLine();
                        }
                    }
                    Console.WriteLine("\n---------------------------------------------------------------------------------------------------");
                }
            }
        }

        #region update operations

        public void updateAgencyes(Agencyes entity)
        {
            string query = $"UPDATE [dbo].[Agencyes] SET [Id] = {entity.Id} ,[Name] = '{entity.Name}',[License] = {entity.License},[Address] ='{entity.Address}' ,[PhoneNumber] ='{entity.PhoneNumber}' ,[DirectorId] = {entity.DirectorId} WHERE Id= {entity.Id};";
            executeQuery(query);
        }

        public void updateBrockers(Brockers entity)
        {
            string query = $"UPDATE [dbo].[Brockers] SET [Id] = {entity.Id},[FirstName] = '{entity.FirstName}' ,[LastName] ='{entity.LastName}' ,[PhoneNumber] = '{entity.PhoneNumber}' ,[AgencyId] = {entity.AgencyId} WHERE Id = {entity.Id};";
            executeQuery(query);
        }

        public void updateInvoices(Invoices entity)
        {
            string query = $"UPDATE [dbo].[Invoices] SET [Id] = {entity.Id},[Date] = '{entity.Date}',[OperationId] = {entity.OperationId} ,[BrockerId] = {entity.BrockerId} ,[Bail] = {entity.Bail},[ClientId] = {entity.ClientId},[RealStateAddress] = '{entity.RealStateAddress}' WHERE Id = {entity.Id};";
            executeQuery(query);
        }

        public void updateOperations(Operations entity)
        {
            string query = $"UPDATE [dbo].[Operations] SET [Id] = {entity.Id} ,[Name] = '{entity.Name}' WHERE Id = {entity.Id}";
            executeQuery(query);
        }

        public void updatePersons(Persons entity)
        {
            string query = $"UPDATE [dbo].[Persons] SET [Id] = {entity.Id },[Name] = '{entity.Name}' ,[RoleId] = {entity.RoleId}  WHERE Id = {entity.Id}";
            executeQuery(query);
        }

        public void updateRealStates(RealStates entity)
        {
            string query = $"UPDATE [dbo].[RealStates] SET [Id] = {entity.Id} ,[RegistrationNumber] = {entity.RegistrationNumber} ,[Address] = '{entity.Address}',[Price] = {entity.Price},[Independency]  = {entity.Independency} ,[TypeId] = {entity.TypeId}  WHERE Id = {entity.Id}";
            executeQuery(query);
        }

        public void updateRealStateTypes(RealStateTypes entity)
        {
            string query = $"UPDATE [dbo].[RealStateTypes] SET [Id] =  {entity.Id},[Name] = '{entity.Name}'  WHERE Id = {entity.Id}";
            executeQuery(query);
        }

        public void updateRoles(Roles entity)
        {
            string query = $"UPDATE [dbo].[Roles] SET [Id] =  {entity.Id},[Name] = ' {entity.Name}'  WHERE Id = {entity.Id}";
            executeQuery(query);
        }

        #endregion update operations

        #region delete operations

        public void deleteAll(string tableName)
        {
            string query = $"Truncate table {tableName} ;";
            executeQuery(query);
        }

        public void delete(int id, string tableName)
        {
            string query = $"Delete from {tableName} where Id= {id};";
            executeQuery(query);
        }

        #endregion delete operations

        #region Add operations

        public void addBrockers(Brockers brocker)
        {
            string query = $"INSERT INTO Brockers (Id ,FirstName ,LastName ,PhoneNumber ,AgencyId) VALUES ({brocker.Id},'{brocker.FirstName}', '{brocker.LastName}', '{brocker.PhoneNumber}',{brocker.AgencyId});";
            executeQuery(query);
        }

        public void addAgencies(Agencyes entity)
        {
            string query = $"INSERT INTO [dbo].[Agencyes] ([Id] ,[Name] ,[License] ,[Address] ,[PhoneNumber] ,[DirectorId]) VALUES  ({entity.Id},'{entity.Name}', {entity.License}, '{entity.Address}','{entity.PhoneNumber}',{entity.DirectorId})";
            executeQuery(query);
        }

        public void addInvoices(Invoices entity)
        {
            string query = $"INSERT INTO [dbo].[Invoices] ([Id] ,[Date] ,[OperationId] ,[BrockerId] ,[Bail] ,[ClientId] ,[RealStateAddress]) VALUES  ({entity.Id},{entity.Date}, {entity.OperationId}, {entity.BrockerId},{entity.Bail},{entity.ClientId},'{entity.RealStateAddress}')";
            executeQuery(query);
        }

        public void addOperations(Operations entity)
        {
            string query = $"INSERT INTO [dbo].[Operations] ([Id] ,[Name]) VALUES ({entity.Id},'{entity.Name}')";
            executeQuery(query);
        }

        public void addPersons(Persons entity)
        {
            string query = $"INSERT INTO [dbo].[Persons] ([Id] ,[Name] ,[RoleId]) VALUES ({entity.Id},'{entity.Name}',{entity.RoleId})";
            executeQuery(query);
        }

        public void addRealStates(RealStates entity)
        {
            string query = $"INSERT INTO [dbo].[RealStates] ([Id] ,[RegistrationNumber] ,[Address] ,[Price] ,[Independency] ,[TypeId]) VALUES ({entity.Id},{entity.RegistrationNumber},'{entity.Address}',{entity.Price},{entity.Independency},{entity.TypeId})";
            executeQuery(query);
        }

        public void addRealStateTypes(RealStateTypes entity)
        {
            string query = $"INSERT INTO [dbo].[RealStateTypes] ([Id] ,[Name]) VALUES ({entity.Id},'{entity.Name}')";
            executeQuery(query);
        }

        public void addRoles(Roles entity)
        {
            string query = $"INSERT INTO [dbo].[Roles] ([Id] ,[Name]) VALUES ({entity.Id},'{entity.Name}')";
            executeQuery(query);
        }

        #endregion Add operations

        private void executeQuery(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Select operations

        public void selectAllBrockers()
        {
            selectFromTable("brockers");
        }

        public void selectAllAgency()
        {
            selectFromTable("Agencyes");
        }

        public void selectAllInvoices()
        {
            selectFromTable("invoices");
        }

        public void selectAllOperations()
        {
            selectFromTable("operations");
        }

        public void selectAllPersons()
        {
            selectFromTable("persons");
        }

        public void selectAllRealStates()
        {
            selectFromTable("realstates");
        }

        public void selectAllRealStateTypes()
        {
            selectFromTable("realstatetypes");
        }

        public void selectAllBrockersRoles()
        {
            selectFromTable("roles");
        }

        private void selectFromTable(string tableName)
        {
            string selectQuery = $"Select * from {tableName}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(selectQuery, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                foreach (DataTable dt in ds.Tables)
                {
                    Console.WriteLine(tableName);
                    Console.WriteLine();
                    foreach (DataRow row in dt.Rows)
                    {
                        var cells = row.ItemArray;
                        foreach (object cell in cells)
                            Console.Write("\t{0}", cell);
                        Console.WriteLine();
                    }
                }
            }
        }

        #endregion Select operations
    }
}