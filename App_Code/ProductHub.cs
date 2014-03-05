using Microsoft.AspNet.SignalR.Hubs;
using WebMatrix.Data;
using System.Collections.Generic;

public class ProductHub : Hub
{
    public void BuyItem(int id, int quantity) {
        using (var db = Database.Open("Northwind")) {
            var ItemsInStock = db.QueryValue("SELECT InStock FROM Items WHERE id = @0", id);
            if (ItemsInStock > 0) {
                db.Execute("UPDATE Items Set InStock = InStock - @1 WHERE id = @0", id, quantity);
                ItemsInStock -= quantity;
            }
            Clients.All.updateAvailableStock(ItemsInStock, id);
        }
    }

    public void AddCategory(string CatName, string description){
        using (var db = Database.Open("Northwind")) {
            db.Execute("INSERT INTO categories (CategoryName,Description_cat) VALUES (@0,@1)", CatName, description);
            var sql = "SELECT * FROM Categories";
            var newData = db.Query(sql);
            Clients.All.updateCategories(newData);
        }}   

        public void removeCategory(int catId){
        using (var db = Database.Open("Northwind")) {
            db.Execute("DELETE FROM categories WHERE CategoryID = @0", catId);
            var sql = "SELECT * FROM Categories";
            var newData = db.Query(sql);
            Clients.All.updateCategories(newData);
        }}

        public void addItem(int catId, string name, string model, string desc, int stock, string location){
                using (var db = Database.Open("Northwind")) {
                    db.Execute("INSERT INTO Items (Name, Model, Description, InStock, Location, CategoryID) VALUES (@0,@1,@2,@3,@4,@5)",name, model, desc, stock, location, catId);
                    var newData = db.Query("SELECT id, Name FROM Items WHERE CategoryID = @0",catId);
                    var newData2 = db.Query("SELECT Name FROM Items");
                    Clients.All.updateItems(newData,newData2);
        }}

        public void removeItem(int id, int catId){
              using (var db = Database.Open("Northwind")) {
              db.Execute("DELETE FROM  items WHERE id = @0",id);
               var newData = db.Query("SELECT id, Name From Items Where CategoryId = @0",catId);
               Clients.All.updateItems(newData);       
        }}

        public void updateItem(int id, string name, string model, string desc, int stock, string location){
        using (var db = Database.Open("Northwind")) {
            db.Execute("UPDATE Items SET Name=@0, Model=@1, Description=@2, InStock=@3, Location=@4 WHERE id=@5",name, model, desc, stock, location, id);
            var newData = db.Query("SELECT * FROM Items WHERE id=@0",id);
            Clients.All.updateItemsData(newData);
        }}

        public void order(string jsonData, string name, string email){
            using (var db = Database.Open("Northwind")) {
                db.Execute("INSERT INTO Orders (Name, Email) VALUES (@0,@1)", name, email);
                var orderID = db.GetLastInsertId(); 
                var rows = Newtonsoft.Json.Linq.JArray.Parse(jsonData);
                var jarray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonData.ParseAccounts>>(jsonData);
                foreach (var x in jarray){
                try{
                    db.Execute("INSERT INTO ItemsOrdered (itemId, item, quantity, orderId) VALUES (@0,@1,@2,@3)", x.id, x.item, x.quantity, orderID);
                }
                catch{}
                }
                var data= db.Query("SELECT Id, name, email, CONVERT(NVARCHAR(20), OrderDate, 100) AS OrderDate FROM Orders WHERE Completed=0");
                Clients.All.updateOrders(data); 
        }}
        
       public void orderComplete(int id){
        using (var db = Database.Open("Northwind")) {
            db.Execute("UPDATE Orders SET Completed = 1 WHERE Id=@0",id);
            var data = db.Query("SELECT Id, name, email, CONVERT(NVARCHAR(20), OrderDate, 100) AS OrderDate FROM Orders WHERE Completed=0");
            Clients.All.updateOrders(data);
        }}
     
}
 
           