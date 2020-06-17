using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    /* В цьому файлі зроблена реалізація патерну Factory Method.
     * Я його вибрав тому, що він надає можливість зручно створювати обєкти в основному класі
     * дозволяючи підкласам змінювати тип створюваних обєктів.
     *
     * Тут створено абстрактний клас ICard - представлення одної карти.
     * Від нього внаслідувано два класи: ArrayCard, ListCard. Це зроблено для того шоб розділити карти
     * по двом колекціям: масив та звязний список. Вони містять метод Show() та value для відображення свого вмісту (номеру карти)
     * Клас ListCard має також обєкт який вказує на наступну карту в колеції.
     * Створено абстрактний клас CardCreator - для створення обєктів типу ICard (з значенням та без)
     * Від нього внаслідувано два класи для створення карти в колеції масиву та в колекції звязного списку.
     * Для зберігання колеції карт створено клас CardCollection (абстрактний) та від нього внаслідувані ArrayCardCollection, ListCardCollection
     * Вони мають обєкт класу CardCreator для створення карт в колоді, та метод Add для додавання нової карти
     * Це все зберігається в класі MainCardCollection яка містить в собі обєкт типу CardCollection.
     * Це дозволяє нам взаємозаміняти класи ArrayCardCollection та ListCardCollection.
     * Цей патерн загалом дозволяє зменшити звязність класів та дозволити підставляти в методи реалізації іінтерфейсів а не конкретні типи.
    */

    internal abstract class CardCreator
    {
        public abstract ICard Create();

        public abstract ICard CreateWithValue(int value);
    }

    internal class ArrayCardCreator : CardCreator
    {
        public override ICard Create()
        {
            return new ArrayCard(null);
        }

        public override ICard CreateWithValue(int value)
        {
            return new ArrayCard(value);
        }
    }

    internal class ListCardCreator : CardCreator
    {
        public override ICard Create()
        {
            return new ListCard(null, null);
        }

        public override ICard CreateWithValue(int value)
        {
            return new ListCard(value, null);
        }
    }

    internal abstract class ICard
    {
        abstract public void Show();

        public int? value { get; set; }
    }

    internal class ArrayCard : ICard
    {
        override public void Show()
        {
            Console.WriteLine("This is array card!");
        }

        public ArrayCard(int? value)
        {
            this.value = value;
        }
    }

    internal class ListCard : ICard
    {
        private ListCard nextCard { get; set; }

        override public void Show()
        {
            Console.WriteLine("This is linked list card!");
        }

        public ListCard(int? value)
        {
            this.value = value;
            nextCard = null;
        }

        public ListCard(int? value, ListCard next)
            : this(value)
        {
            nextCard = next;
        }
    }

    internal abstract class CardCollection
    {
        public string name;
        public List<ICard> Items;

        public abstract void Add(int value);
    }

    internal class ArrayCardCollection : CardCollection
    {
        private CardCreator creator;

        public ArrayCardCollection()
        {
            this.creator = new ArrayCardCreator();
            this.name = "Array!";
            this.Items = new List<ICard>();
            Console.WriteLine("Array card collection builded!");
        }

        public override void Add(int value)
        {
            this.Items.Add(creator.CreateWithValue(value));
        }
    }

    internal class ListCardCollection : CardCollection
    {
        private CardCreator creator;

        public ListCardCollection()
        {
            this.creator = new ListCardCreator();
            this.name = "Linked list!";
            this.Items = new List<ICard>();
            Console.WriteLine("List card collection builded!");
        }

        public override void Add(int value)
        {
            this.Items.Add(creator.CreateWithValue(value));
        }
    }

    internal class MainCardCollection
    {
        public CardCollection Cards { get; set; }

        public MainCardCollection(CardCollection collection)
        {
            this.Cards = collection;
        }
    }

    public class Game
    {
        public static void OnMain()
        {
            MainCardCollection cardCollection = new MainCardCollection(new ListCardCollection());
            cardCollection.Cards.Add(2);
            cardCollection.Cards.Add(3);
            cardCollection.Cards.Add(4);
            cardCollection.Cards.Add(5);
            foreach (var item in cardCollection.Cards.Items)
            {
                Console.WriteLine($"Card value: {item.value}");
            }
            Console.WriteLine("--------------------");
            MainCardCollection secondCollection = new MainCardCollection(new ArrayCardCollection());
            secondCollection.Cards.Add(9);
            secondCollection.Cards.Add(8);
            secondCollection.Cards.Add(7);
            secondCollection.Cards.Add(6);
            foreach (var item in secondCollection.Cards.Items)
            {
                Console.WriteLine($"Card value: {item.value}");
            }
        }
    }
}