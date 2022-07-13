using GraphQL;
using GraphQL.Builders;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using My.Shared;

namespace Northwind.GraphQL;

public class NorthwindQuery : ObjectGraphType {
    public NorthwindQuery(NorthwindContext db) {
        Field<ListGraphType<CategoryType>>(
            name: "categories",
            description: "a query type that returns a list of all categories",
            resolve: context => db.Categories.Include(c => c.Products)
        );

        Field<CategoryType>(
            name: "category",
            description: "a query type that returns a category using id",
            arguments: new QueryArguments(
                new QueryArgument<IntGraphType> { Name = "categoryId" }),
            resolve: context =>
            {
                Category? category = db.Categories.Find(context.GetArgument<int>("categoryId"));
                db.Entry(category).Collection(c => c.Products).Load();
                return category;
            }
        );

        Field<ListGraphType<ProductType>>(
            name: "products",            
            arguments: new QueryArguments(
                new QueryArgument<IntGraphType> { Name = "categoryId" }),
            resolve: context =>
            {
                Category? category = db.Categories.Find(context.GetArgument<int>("categoryId"));
                db.Entry(category).Collection(c => c.Products).Load();
                return category.Products;
            }
        );
    }
}