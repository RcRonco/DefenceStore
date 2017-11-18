SELECT ProductID, Price, Quantity, Price*Quantity 
FROM dbo.Orders 
	JOIN dbo.OrderProducts ON dbo.Orders.ID = dbo.OrderProducts.OrderID 
	JOIN dbo.Products ON dbo.OrderProducts.ProductID = dbo.Products.ID 
WHERE dbo.Orders.ID = 1;

SELECT  SUM(Price*Quantity)
FROM dbo.Orders 
	JOIN dbo.OrderProducts ON dbo.Orders.ID = dbo.OrderProducts.OrderID 
	JOIN dbo.Products ON dbo.OrderProducts.ProductID = dbo.Products.ID 
WHERE dbo.Orders.ID = 1;