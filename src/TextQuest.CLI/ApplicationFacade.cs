using System.Text;
using TextQuest.Application.Interfaces;
using TextQuest.Application.Models;
using TextQuest.CLI.Interfaces;
using TextQuest.Domain.Objects;

namespace TextQuest.CLI
{
    internal class ApplicationFacade : IApplication
    {
        private readonly IWorldProvider worldProvider;
        private readonly IPlayerController playerController;
        private readonly StringBuilder stringBuilder;

        private IList<Quest> allQuests;

        public ApplicationFacade(IWorldProvider worldProvider, IPlayerController playerController)
        {
            this.worldProvider = worldProvider;
            this.playerController = playerController;
            stringBuilder = new StringBuilder();
        }

        public void Run()
        {
            var creationParams = new WorldCreationParams(20..40, 4, 6);
            stringBuilder.AppendLine("Генерация...");
            FlushToConsole();

            worldProvider.CreateNew(creationParams);
            playerController.MoveTo(worldProvider.World.Locations[0]);

            allQuests = worldProvider.World.Locations
                .SelectMany(x => x.Characters).SelectMany(x => x.Quests).ToList();

            do
            {
                MainMenu();
            } while (CompletedQuest < allQuests.Count);

            Console.WriteLine("Игра пройдена!");
            Console.ReadLine();
        }

        private int CompletedQuest => allQuests.Count(x => x.Completed);

        private void MainMenu()
        {
            stringBuilder
                .AppendLine($"Мир: \"{worldProvider.World.Name}\"")
                .AppendLine($"Текущая локация: \"{playerController.CurrentLocation.Name}\"")
                .AppendLine($"Прогресс: {CompletedQuest}/{allQuests.Count}")
                .AppendLine();

            ShowActiveQuests();
            ShowPlayerItems();

            stringBuilder.AppendLine("Действия: ")
                .AppendLine("\t0 Изменить локацию")
                .AppendLine("\t1 Взаимодействовать с пресонажем");

            FlushToConsole();
            var selection = GetSelection(2);
            switch (selection)
            {
                case 0:
                    ChangeLocationMenu();
                    break;
                case 1:
                    CharacterInteractMenu();
                    break;
            }
        }
        private void ShowActiveQuests()
        {
            stringBuilder.AppendLine("<<<Активные квесты>>")
                .AppendLine();
            foreach (var quest in playerController.Quests)
            {
                ShowQuest(quest);
            }
        }

        private void ShowQuest(Quest quest)
        {
            stringBuilder.AppendLine("<<Квест>>")
                    .AppendLine($"Для {quest.Giver.Name}")
                    .AppendLine($"Из \"{quest.Giver.Location.Name}\"")
                    .AppendLine("Необходимо: ");
            ShowItemList(quest.RequiredItems);

            stringBuilder.AppendLine("Награда: ");
            ShowItemList(quest.ObtainedItems);
            stringBuilder.AppendLine();
        }

        private void ShowPlayerItems()
        {
            stringBuilder.AppendLine("<<<Инвентарь>>>");
            ShowItemList(playerController.Items);
            stringBuilder.AppendLine();
        }

        private void ShowItemList(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                stringBuilder.AppendLine($"\t- \"{item.Name}\"");
            }
        }
        private void ChangeLocationMenu()
        {
            stringBuilder.AppendLine("<<<Смена локации>>>")
                .AppendLine("\t0 Отмена")
                .AppendJoin("\r\n", worldProvider.World.Locations.Select(
                    (x, i) => $"\t{i + 1} Переход в \"{x.Name}\""));

            FlushToConsole();
            var selected = GetSelection(worldProvider.World.Locations.Count);
            if (selected != 0)
            {
                selected--;
                playerController.MoveTo(worldProvider.World.Locations[selected]);
            }
        }

        private void CharacterInteractMenu() { }

        private int GetSelection(int variantCount)
        {
            bool selected = false;
            int selection;
            do
            {
                Console.Write("Выбор: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out selection))
                {
                    if (selection >= 0 && selection < variantCount)
                    {
                        selected = true;
                    }
                    else
                    {
                        Console.WriteLine($"Значение вне диапазон! Допустимый диапазон: 0..{variantCount - 1}");
                    }
                }
                else
                {
                    Console.WriteLine("Значение должно быть целым числом");
                }
            } while (!selected);
            return selection;
        }

        private void FlushToConsole()
        {
            var text = stringBuilder.ToString();
            Console.Clear();
            Console.WriteLine(text);
            stringBuilder.Clear();
        }
    }
}
