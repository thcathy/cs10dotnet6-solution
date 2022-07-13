using GraphQL.Types;
using My.Shared;

namespace Northwind.GraphQL;

public class CategoryType : ObjectGraphType<Category> {
    public CategoryType() {
        Name = "Category";
        Field(c => c.CategoryId).Description("id of category");
        Field(c => c.CategoryName).Description("name of category");
        Field(c => c.Description).Description("desc of category");
        Field(c => c.Products, type: typeof(ListGraphType<ProductType>)).Description("name of category");
    }
}