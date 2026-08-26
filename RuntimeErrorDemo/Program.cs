Console.Write("Enter quantity: ");

string? input = Console.ReadLine();

// Intentional coding-test failure: the compiler cannot know what text the user enters.
int quantity = int.Parse(input!);

Console.WriteLine($"Quantity: {quantity}");
