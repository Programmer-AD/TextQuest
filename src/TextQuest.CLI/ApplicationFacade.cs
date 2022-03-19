using System.Text;
using TextQuest.Application.Exceptions;
using TextQuest.Application.Interfaces;
using TextQuest.Application.Models;
using TextQuest.CLI.Interfaces;
using TextQuest.Domain.Common;
using TextQuest.Domain.Objects;

namespace TextQuest.CLI
{
    internal class ApplicationFacade : IApplication
    {
        private readonly IWorldProvider worldProvider;
        private readonly IPlayerController playerController;
        private readonly StringBuilder stringBuilder;
        private readonly StringBuilder extraMessageStringBuilder;

        private World World => worldProvider.World;
        private Location PlayerLocation => playerController.CurrentLocation;

        public ApplicationFacade(IWorldProvider worldProvider, IPlayerController playerController)
        {
            this.worldProvider = worldProvider;
            this.playerController = playerController;
            stringBuilder = new();
            extraMessageStringBuilder = new();
        }

        public void Run()
        {
            var creationParams = new WorldCreationParams(20..40, 4, 6);
            stringBuilder.AppendLine("Генерация...");
            FlushToConsole();

            worldProvider.CreateNew(creationParams);
            playerController.MoveTo(World.Locations[0]);

            do
            {
                MainMenu();
            } while (World.CompletedQuestCount < World.QuestCount);

            PrintExtraMessage("Игра пройдена!");
        }

        private void MainMenu()
        {
            stringBuilder
                .AppendLine($"Мир: \"{World.Name}\"")
                .AppendLine($"Текущая локация: \"{PlayerLocation.Name}\"" +
                $" ({PlayerLocation.CompletedQuestCount}/{PlayerLocation.QuestCount})")
                .AppendLine($"Прогресс: {World.CompletedQuestCount}/{World.QuestCount}")
                .AppendLine();

            ShowActiveQuests();
            ShowPlayerItems();

            stringBuilder.AppendLine("Действия: ")
                .AppendLine("\t0 Изменить локацию")
                .AppendLine("\t1 Взаимодействовать с персонажами");

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
            stringBuilder.AppendLine("<<<Активные квесты>>");

            foreach (var quest in playerController.Quests)
            {
                ShowQuest(quest);
                stringBuilder.AppendLine();
            }
        }

        private void ShowQuest(Quest quest)
        {
            stringBuilder.AppendLine("<<Квест>>")
                    .AppendLine($"Для {quest.Giver.Name}")
                    .AppendLine($"Из \"{quest.Giver.Location.Name}\"");

            stringBuilder.AppendLine("Необходимо: ");
            ShowItemList(quest.RequiredItems);
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Награда: ");
            ShowItemList(quest.ObtainedItems);
        }

        private void ShowPlayerItems()
        {
            stringBuilder.AppendLine("<<<Инвентарь>>>");
            ShowItemList(playerController.Items);
            stringBuilder.AppendLine();
        }

        private void ShowItemList(IEnumerable<Counted<Item>> items)
        {
            foreach (var (item, count) in items)
            {
                stringBuilder.AppendLine($"\t- {count} * \"{item.Name}\"");
            }
        }
        private void ChangeLocationMenu()
        {
            stringBuilder.AppendLine("<<<Смена локации>>>")
                .AppendLine("\t0 Отмена")
                .AppendJoin("\r\n", World.Locations.Select(
                    (x, i) => $"\t{i + 1} Переход в \"{x.Name}\" ({x.CompletedQuestCount}/{x.QuestCount})"));

            FlushToConsole();
            var selected = GetSelection(World.Locations.Count + 1);
            if (selected != 0)
            {
                var newLocation = World.Locations[selected - 1];
                playerController.MoveTo(newLocation);
            }
        }

        private void CharacterInteractMenu()
        {
            int selected;
            do
            {
                stringBuilder.AppendLine("<<<Взаимодействие с персонажами>>>")
                    .AppendLine("\t0 Отмена")
                    .AppendJoin(Environment.NewLine, playerController.CurrentLocation.Characters.Select(
                        (x, i) => $"\t{i + 1} Взаимодействовать с {x.Name} ({x.CompletedQuestCount}/{x.QuestCount})"));

                FlushToConsole();
                selected = GetSelection(playerController.CurrentLocation.Characters.Count + 1);
                if (selected != 0)
                {
                    var character = playerController.CurrentLocation.Characters[selected - 1];
                    ShowInteractionMenu(character);
                }
            } while (selected != 0);
        }

        private void ShowInteractionMenu(Character character)
        {
            int selection;

            do
            {
                stringBuilder.AppendLine($"<<<Взаимодействие с {character.Name}>>>")
                    .AppendLine("\t0 Отмена")
                    .AppendLine("\t1 Взять все доступные квесты")
                    .AppendLine("\t2 Обмен предметами");

                FlushToConsole();

                selection = GetSelection(3);
                switch (selection)
                {
                    case 1:
                        PickAvailableQuests(character);
                        break;
                    case 2:
                        ExchangeItems(character);
                        break;
                }
            } while (selection != 0);
        }

        private void PickAvailableQuests(Character character)
        {
            if (character.CompletedQuestCount < character.QuestCount)
            {
                var availableQuests = character.AvailableQuests;
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
                    PrintExtraMessage($"В журнал добавлено {pickedCount} новых квестов");
                }
                else
                {
                    var recomendedGiver = character.RecomendedQuest.Giver;
                    PrintExtraMessage("Нет доступных квестов" + Environment.NewLine +
                        $"Подсказка: сходите к {recomendedGiver.Name} из {recomendedGiver.Location.Name}");
                }
            }
            else
            {
                PrintExtraMessage("Все квесты выполнены!");
            }
        }

        private void ExchangeItems(Character character)
        {
            try
            {
                playerController.ExchangeQuestItems(character);
                PrintExtraMessage("Обмен успешен");
            }
            catch (ItemExchangeException)
            {
                PrintExtraMessage("Нет предметов или повода для обмена");
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
                    Console.WriteLine("Значение должно быть целым числом!");
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

        private void PrintExtraMessage(string text)
        {
            extraMessageStringBuilder.Clear()
                .AppendLine()
                .AppendLine("[Информация]")
                .AppendLine(text)
                .AppendLine()
                .Append("(Нажмите Enter для продолжения)");

            Console.Write(extraMessageStringBuilder.ToString());
            Console.ReadLine();
        }
    }
}
