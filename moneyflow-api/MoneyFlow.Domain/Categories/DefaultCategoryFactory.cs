using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Categories;

public static class DefaultCategoryFactory
{
    public static IReadOnlyList<Category> CreateForUser(long userId)
    {
        return
        [
            new Category(
                userId,
                "Alimentación",
                TransactionType.Expense,
                "utensils"),

            new Category(
                userId,
                "Transporte",
                TransactionType.Expense,
                "bus"),

            new Category(
                userId,
                "Vivienda",
                TransactionType.Expense,
                "house"),

            new Category(
                userId,
                "Servicios",
                TransactionType.Expense,
                "lightbulb"),

            new Category(
                userId,
                "Salud",
                TransactionType.Expense,
                "heart-pulse"),

            new Category(
                userId,
                "Educación",
                TransactionType.Expense,
                "graduation-cap"),

            new Category(
                userId,
                "Entretenimiento",
                TransactionType.Expense,
                "gamepad-2"),

            new Category(
                userId,
                "Compras",
                TransactionType.Expense,
                "shopping-bag"),

            new Category(
                userId,
                "Otros",
                TransactionType.Expense,
                "ellipsis"),

            new Category(
                userId,
                "Sueldo",
                TransactionType.Income,
                "wallet-cards"),

            new Category(
                userId,
                "Freelance",
                TransactionType.Income,
                "laptop"),

            new Category(
                userId,
                "Bonos",
                TransactionType.Income,
                "gift"),

            new Category(
                userId,
                "Inversiones",
                TransactionType.Income,
                "chart-no-axes-combined"),

            new Category(
                userId,
                "Otros ingresos",
                TransactionType.Income,
                "circle-plus")
        ];
    }
}