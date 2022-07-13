using GraphQL.Types;
using My.Shared;

namespace Northwind.GraphQL;

public class ProductType : ObjectGraphType<Product> {
    public ProductType() {
        Name = "Product";
        Field(p => p.ProductId).Description("p.ProductId");
        Field(p => p.ProductName).Description("p.ProductName");
        Field(p => p.CategoryId, type: typeof(IntGraphType)).Description("p.CategoryId");
        Field(p => p.Category, type: typeof(CategoryType)).Description("p.Category");
        Field(p => p.UnitPrice, type: typeof(DecimalGraphType)).Description("p.UnitPrice");
        Field(p => p.UnitsInStock, type: typeof(IntGraphType)).Description("p.UnitsInStock");
        Field(p => p.UnitsOnOrder, type: typeof(IntGraphType)).Description("p.UnitsOnOrder");
    }
}