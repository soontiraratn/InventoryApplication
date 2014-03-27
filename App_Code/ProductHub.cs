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
            db.Execute("INSERT INTO categories (CategoryName,DescriptionCat) VALUES (@0,@1)", CatName, description);
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

        public void editCategory(string cat, string catDesc, int catId){
        using (var db = Database.Open("Northwind")) {
            db.Execute("UPDATE categories SET CategoryName=@0, DescriptionCat=@1 WHERE CategoryID = @2", cat, catDesc, catId);
            var sql = "SELECT * FROM Categories";
            var newData = db.Query(sql);
            Clients.All.updateCategories(newData);
        }}

        public void addItem(int catId, int catId2, string name, string model, string desc, int stock, string location){
                using (var db = Database.Open("Northwind")) {
                    db.Execute("INSERT INTO Items (Name, Model, Description, InStock, Location, CategoryID) VALUES (@0,@1,@2,@3,@4,@5)",name, model, desc, stock, location, catId);
                    var data = db.Query("SELECT id, Name FROM Items WHERE CategoryID = @0",catId);
                    var data3 = db.Query("SELECT id, Name FROM Items WHERE CategoryID = @0",catId2);
                    var allData = db.Query("SELECT Name,CategoryID  FROM Items");
                    Clients.All.updateItems(data,allData,data3);
        }}

        public void removeItem(int id, int catId){
              using (var db = Database.Open("Northwind")) {
              db.Execute("DELETE FROM  items WHERE id = @0",id);
               var data = db.Query("SELECT id, Name From Items Where CategoryId = @0",catId);
               Clients.All.updateItems3(data);       
        }}

        public void updateItem(int id, string name, string model, string desc, int stock, string location){
        using (var db = Database.Open("Northwind")) {
            db.Execute("UPDATE Items SET Name=@0, Model=@1, Description=@2, InStock=@3, Location=@4 WHERE id=@5",name, model, desc, stock, location, id);
            var newData = db.Query("SELECT * FROM Items WHERE id=@0",id);
            //Clients.All.updateItemsData(newData);
            Clients.All.updateItems3(newData);
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
                // Send email
                var emailAlert = "Darwin.Soontiraratn@tax.hctx.net";
                System.Web.Helpers.WebMail.Send(to: emailAlert,
                    subject: "New Order Submitted. Mary Gil's List",
                    body: "Order Submitted by " + name + ". <br />Order Number " + orderID + "<br /> <a href='http://svptaxweb1/Maryslist/admin'>View Order</a>"
                );
                System.Web.Helpers.WebMail.Send(to: email,
                    subject: "Order Confirmation from The Mary Gil List",
                    body: "Hey there " + name + ". Just letting you know we got your order. <br />Your Order Number is " + orderID + "<br />You will be contacted soon about recieving your order. "
                );
            }}
        
       public void orderComplete(int id){
        using (var db = Database.Open("Northwind")) {
            db.Execute("UPDATE Orders SET Completed = 1 WHERE Id=@0",id);
            var data = db.Query("SELECT Id, name, email, CONVERT(NVARCHAR(20), OrderDate, 100) AS OrderDate FROM Orders WHERE Completed=0");
            Clients.All.updateOrders(data);
        }}
     
}
 
           