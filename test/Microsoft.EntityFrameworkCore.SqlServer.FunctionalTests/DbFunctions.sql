IF object_id(N'[EmployeeOrderCount]', N'FN') IS NOT NULL
    DROP FUNCTION [EmployeeOrderCount]
GO

create function [dbo].[EmployeeOrderCount] (@employeeId int)
returns int
as
begin
	return (select count(orderId) from orders where employeeid = @employeeId);
end
GO


IF object_id(N'[IsTopEmployee]', N'FN') IS NOT NULL
    DROP FUNCTION [IsTopEmployee]
GO

create function [dbo].[IsTopEmployee] (@employeeId int)
returns bit
as
begin
	if(@employeeId = 4 or @employeeId = 5 or @employeeId = 8)
		return 1
		
	return 0
end
GO


IF object_id(N'[GetEmployeeWithMostOrdersAfterDate]', N'FN') IS NOT NULL
    DROP FUNCTION [GetEmployeeWithMostOrdersAfterDate]
GO

create function [dbo].[GetEmployeeWithMostOrdersAfterDate] (@searchDate Date)
returns int
as
begin
	return (select top 1 employeeId
			from orders
			where orderDate > @searchDate
			group by EmployeeID
			order by count(orderid) desc)
end
GO

IF object_id(N'[GetReportingPeriodStartDate]', N'FN') IS NOT NULL
    DROP FUNCTION [GetReportingPeriodStartDate]
GO

create function [dbo].[GetReportingPeriodStartDate] (@period int)
returns DateTime
as
begin
	return '1/1/1998'
end
GO


IF object_id(N'[StarValue]', N'FN') IS NOT NULL
    DROP FUNCTION [StarValue]
GO

create function [dbo].[StarValue] (@starCount int, @value nvarchar(max))
returns  nvarchar(max)
as
begin
	return replicate('*', @starCount) + @value
end
GO

IF object_id(N'[AddValues]', N'FN') IS NOT NULL
    DROP FUNCTION [AddValues]
GO

create function [dbo].[AddValues] (@a int, @b int)
returns  int
as
begin
	return @a + @b
end
GO

IF object_id(N'[GetBestYearEver]', N'FN') IS NOT NULL
    DROP FUNCTION [GetBestYearEver]
GO

create function [dbo].[GetBestYearEver] ()
returns datetime
as
begin
	return '1/1/1998'
end
GO


IF object_id(N'[FindReportsForManager]', N'TF') IS NOT NULL
    DROP FUNCTION [FindReportsForManager]
GO

create function [dbo].FindReportsForManager()
returns @reports table
(
	EmployeeId int not null,
	ManagerId int null
)
as
begin
	with FindEmploeeHeirarchyCTE as
	(
		SELECT EmployeeID, ReportsTo    
		FROM Employees 
		WHERE ReportsTo IS NULL 
	
		UNION ALL 

		SELECT e.employeeID, e.ReportsTo 
		FROM Employees e INNER JOIN FindEmploeeHeirarchyCTE h  
		ON e.ReportsTo = h.employeeID 
	)

	insert into @reports
	select EmployeeID, ReportsTo
	from FindEmploeeHeirarchyCTE

	return 
end
go

IF object_id(N'[GetEmployeeOrderCountByYear]', N'TF') IS NOT NULL
    DROP FUNCTION [GetEmployeeOrderCountByYear]
GO

create function [dbo].GetEmployeeOrderCountByYear(@employeeId int)
returns @reports table
(
	OrderCount int not null,
	Year int not null
)
as
begin
	
	insert into @reports
	select count(orderId), year(orderDate)
	from orders
	where employeeId = @employeeId
	group by employeeId, year(orderDate)
	order by year(orderDate)
	
	return 
end
go


IF object_id(N'[GetTopThreeSellingProducts]', N'TF') IS NOT NULL
    DROP FUNCTION [GetTopThreeSellingProducts]
GO

create function [dbo].GetTopThreeSellingProducts()
returns @products table
(
	ProductId int not null,
	AmountSold int
)
as
begin
	
	insert into @products
	select top 3 ProductID, sum(quantity) as totalSold
	from [order details]
	group by ProductID
	order by totalSold desc

	return 
end
go


IF object_id(N'[GetTopThreeSellingProductsForYear]', N'TF') IS NOT NULL
    DROP FUNCTION [GetTopThreeSellingProductsForYear]
GO

create function [dbo].GetTopThreeSellingProductsForYear(@salesYear dateTime)
returns @products table
(
	ProductId int not null,
	AmountSold int
)
as
begin
	
	insert into @products
	select top 3 ProductID, sum(quantity) as totalSold
	from [order details] od
	join orders o on od.orderId = o.orderId
	where year(o.orderDate) = year(@salesYear)
	group by ProductID
	order by totalSold desc

	return 
end
go



IF object_id(N'[GetLatestNOrdersForCustomer]', N'TF') IS NOT NULL
    DROP FUNCTION [GetLatestNOrdersForCustomer]
GO

create function [dbo].GetLatestNOrdersForCustomer(@lastNOrders int, @customerId nvarchar(5))
returns @products table
(
	OrderId int,
	OrderDate datetime
)
as
begin
	
	insert into @products
	select top(@lastNOrders) orderId, orderDate
	from orders
	where orders.customerid = @customerId
	order by orderDate desc

	return 
end
go
