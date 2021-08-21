using System;
using System.Collections.Generic;
using System.Linq;

namespace MonopolyCoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class RentField : Field
    {
        public RentField(string name, int commonRent, int[] houseRents, int deposit) : base(name)
        {
            Name = name;
            CommonRent = commonRent;
            HouseRents = houseRents;
            Deposit = deposit;
        }

        public object Group;

        public object Color;

        public int CommonRent;

        public int[] HouseRents;

        public int Deposit;
    }

    public class Field
    {
        public Field()
        {
        }

        public Field(string name, object image)
        {
            Name = name;
            Image = image;
        }

        public Field(string name)
        {
            Name = name;
        }

        public string Name;

        public object Image;

        public delegate void PlayerHandler(Player player, MonopolyGame game);

        public event PlayerHandler PlayerCame;

        public void OnPlayerCame(Player player, MonopolyGame game) => PlayerCame?.Invoke(player, game);
    }

    public class Player
    {
        public int Balance;

        public int FieldPosition;

        public string Name;

        public Player(int balance, string name)
        {
            Balance = balance;
            Name = name;
        }

        public List<RentField> ActiveProperty;

        public List<RentField> DepositProperty;
    }

    public class Card
    {
        public Card(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public string Name;

        public string Message;

        public event Field.PlayerHandler ActivateCard;
    }

    public class Dice
    {
        private Random generator;
        private int count;
        private int maxValue;

        public Dice(int count, int maxValue)
        {
            generator = new Random();
            this.count = count;
            this.maxValue = maxValue;
        }

        public int ThrowDice(out bool isMatching)
        {
            List<int> current = new List<int>();
            for (int i = 0; i < count; i++)
            {
                current.Add(generator.Next(1, maxValue + 1));
            }

            isMatching = false || current.All(j => current[0] == j);
            return current.Sum();
        }
    }

    public class MonopolyGame
    {
        public Dice Dice;

        private int _playerIndex = 0;

        public List<Player> Players;

        public List<Field> GameField;

        public MonopolyGame(List<Field> corners, List<Field> commonFields, List<Player> players)
        {
            if (commonFields.Count % corners.Count != 0)
            {
                throw new ArgumentException("Количество сторон не кратно количеству угов");
            }

            Players = players;

            Dice = new Dice(2, 6);
            int sideLen = commonFields.Count / corners.Count;
            GameField = new List<Field>();
            GameField.AddRange(commonFields);
            for (int i = 0; i < corners.Count; i++)
            {
                GameField.Insert(i * sideLen + 1, corners[i]);
            }
        }

        public void NextMove(int reference)
        {
            var steps = Dice.ThrowDice(out bool isMatching);
            if (reference == 3)
            {
                Players[_playerIndex].FieldPosition =
                    GameField.IndexOf(GameField.FirstOrDefault(i => i.Name == "Турьма"));
            }

            Players[_playerIndex].FieldPosition = (Players[_playerIndex].FieldPosition + steps) % GameField.Count;

            Field currentField = GameField[Players[_playerIndex].FieldPosition];
            currentField.OnPlayerCame(Players[_playerIndex], this);
            if (isMatching)
            {
                NextMove(reference + 1);
            }

            _playerIndex = (_playerIndex + 1) % Players.Count;
        }
    }
}