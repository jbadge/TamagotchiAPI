using System;

namespace TamagotchiAPI
{
    public class Menu
    {
        public static void WelcomeMessage()
        {





            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@"            Welcome to...");
            string text = """                                                                     
                                                                                              88          88
  ,d                                                                           ,d             88          ""
  88                                                                           88             88
MM88MMM,adPPYYba,88,dPYba,,adPYba, ,adPPYYba, ,adPPYb,d8 ,adPPYba,  ,adPPYba,MM88MMM ,adPPYba,88,dPPYba,  88
  88   ""     `Y888P'   "88"    "8a""     `Y8a8"    `Y88a8"     "8aa8"     "8a 88   a8"     ""88P'    "8a 88
  88   ,adPPPPP8888      88      88,adPPPPP888b       888b       d88b       d8 88   8b        88       88 88
  88,  88,    ,8888      88      8888,    ,88"8a,   ,d88"8a,   ,a8""8a,   ,a8" 88   "8a,   ,aa88       88 88
  "Y888`"8bbdP"Y888      88      88`"8bbdP"Y8 `"8bbdP"Y8 `"YbbdP"Y8 `"Ybbd8"'  "Y888 `"Ybbd8"'88       88 88
                                                         aa,    ,88
                                                          "Y8bbdP"
""";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
            //             //             Console.WriteLine("Press any key to continue...");
            //             //             Console.ReadKey(true).Key.ToString();
            //             //             Console.Clear();
        }

        public static void MainMenu()
        {
            WelcomeMessage();

            bool usingMenu = true;

            while (usingMenu)
            {
                var menuInput = PromptForInt("1. See all pets\n2. Select a pet to interact with\n3. Create a pet\n4. Delete a pet\n5. Press any other key to exit the program");

                switch (menuInput)
                {
                    case 1:
                        SeeAllPets();
                        break;
                    case 2:
                        SelectPet();
                        break;
                    case 3:
                        CreatePet();
                        break;
                    case 4:
                        DeletePet();
                        break;
                    default:
                        usingMenu = false;
                        break;
                }
                // See all pets
                // Select pet

                // Create new pet
                // Delete a pet
            }
        }

        public static void SeeAllPets()
        {

        }

        public static void SelectPet()
        {
            // Play with pet // Add timer to how often pet can be played with
            // Feed pet // Add timers to how often pet can be fed
            // Scold pet // ??
        }

        public static void CreatePet()
        {

        }

        public static void DeletePet()
        {

        }

        public static int PromptForInt(string prompt)
        {
            var inputWasInteger = false;
            var inputAsInteger = 0;

            while (!inputWasInteger)
            {
                var userInput = PromptForString(prompt);
                var isThisGoodInput = int.TryParse(userInput, out inputAsInteger);

                if (isThisGoodInput == true)
                {
                    inputWasInteger = true;
                }
                else
                {
                    Console.WriteLine("This is not a valid number. Please try again");
                    DialogueRefresher();
                }
            }
            return inputAsInteger;
        }

        public static string PromptForString(string prompt)
        {
            Console.WriteLine(prompt);
            var userInput = Console.ReadLine();
            Console.Clear();
            return userInput;
        }

        public static void DialogueRefresher()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true).Key.ToString();
            Console.Clear();
        }

    }
}