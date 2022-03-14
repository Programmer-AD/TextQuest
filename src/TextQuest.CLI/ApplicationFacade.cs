using System.Text;
using TextQuest.Application.Exceptions;
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
            var selected = GetSelection(worldProvider.World.Locations.Count + 1);
            if (selected != 0)
            {
                var newLocation = worldProvider.World.Locations[--selected];
                playerController.MoveTo(newLocation);
            }
        }

        private void CharacterInteractMenu()
        {
            int selected;
            do
            {
                stringBuilder.AppendLine("<<<Взаимодействие с пресонажами>>>")
                    .AppendLine("\t0 Отмена")
                    .AppendJoin("\r\n", playerController.CurrentLocation.Characters.Select(
                        (x, i) => $"\t{i + 1} Взаимодействовать с {x.Name} {(x.Quests.All(x => x.Completed) ? "(Всё выполнено)" : "")}"));

                FlushToConsole();
                selected = GetSelection(playerController.CurrentLocation.Characters.Count + 1);
                if (selected != 0)
                {
                    var character = playerController.CurrentLocation.Characters[--selected];
                    ShowInteractionMenu(character);
                }
            } while (selected != 0);
        }

        private void ShowInteractionMenu(Character character)
        {
            stringBuilder.AppendLine($"<<<Взаимодействие с {character.Name}>>>")
                .AppendLine("\t0 Отмена")
                .AppendLine("\t1 Взять все доступные квесты")
                .AppendLine("\t2 Обмен предметами");

            FlushToConsole();
            var selection = GetSelection(3);
            switch (selection)
            {
                case 1:
                    PickAvailableQuests(character);
                    break;
                case 2:
                    ExchangeItems(character);
                    break;
            }
        }

        private void PickAvailableQuests(Character character)
        {
            var notCompletedQuests = character.Quests.Where(x => !x.Completed);
            if (notCompletedQuests.Any())
            {
                var availableQuests = notCompletedQuests.Where(
                    x => x.RequiredQuests.All(x => x.Completed));

                if (availableQuests.Any())
                {
                    int pickedCount = 0;
                    foreach (var quest in availableQuests)
                    {
                        try
                        {
                            playerController.PickQuest(quest);
                            pickedCount++;
                        }
                        catch (QuestAddingException) { }
                    }
                    Console.WriteLine($"В журнал добавлено {pickedCount} новых квестов");
                    Console.ReadLine();
                }
                else
                {
                    var questRecomendation = notCompletedQuests.First()
                        .RequiredQuests.First(x => !x.Completed);
                    Console.WriteLine("Нет доступных квестов");
                    Console.WriteLine($"Подсказка: сходите к {questRecomendation.Giver.Name} из {questRecomendation.Giver.Location.Name}");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Все квесты выполнены!");
                Console.ReadLine();
            }
        }

        private void ExchangeItems(Character character)
        {
            try
            {
                playerController.ExchangeQuestItems(character);
            }
            catch (ItemExchangeException)
            {
                Console.WriteLine("Нет предметов или повода для обмена");
                Console.ReadLine();
            }
        }

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
