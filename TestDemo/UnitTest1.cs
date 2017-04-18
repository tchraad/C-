using System;

using NUnit.Framework;
using NUnit.Framework.Internal;
using Moq;


namespace TestDemo
{
    
    public class UnitTest1
    {
        // Factory
        private StringCalculator GetCalculator()
        {
            var calc = new StringCalculator();
            return calc;
        }

        [Test]
        public void TestMethod1()
        {
            StringCalculator calc = GetCalculator();
            int expectedResult = 0;
            int result = calc.Add("");
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(1,"1")]
        [TestCase(2, "2")]
        [TestCase(3, "3")]
        public void Add_SingleNumbers_ReturnTheNumbers(int expected,string input)
        {
            StringCalculator calc = GetCalculator();
            //var expectedResult = 3;
            int result = calc.Add(input);
            Assert.AreEqual(expected, result);
        }

        [TestCase("2,3",5)]
        [TestCase("101,3", 104)]
        public void Add_MultipleNumbers_SumAllNumbers(string input,int expectedResult)
        {
            StringCalculator calc = GetCalculator();
            int result = calc.Add(input);            
            Assert.AreEqual(expectedResult, result);
        }       

        [Test]
        public void Add_invalidString_ThrowsException()
        {
            var input = "a,1";
            StringCalculator calc = GetCalculator();
            
            var ex = Assert.Throws<ArgumentException>(() => calc.Add(input));
            Assert.IsTrue(ex.Message.Contains("Input format is incorrect"));
            
        }

        [Test]
        public void Add_InvalidSeparator_ThrowsException()
        {
            var input = "1;1";
            StringCalculator calc = GetCalculator();

            var ex = Assert.Throws<ArgumentException>(() => calc.Add(input));
            Assert.IsTrue(ex.Message.Contains("Separator is incorrect"));
        }


        [Test]
        public void Add_ResultIsAPrimeNumber_resultAreSaved()
        {
            Mock<IStore> mockstore = new Mock<IStore>();
            StringCalculator calc = new StringCalculator(mockstore.Object);
            var result = calc.Add("3,4");
            mockstore.Verify(m => m.Save(It.IsAny<int>()), Times.Once);
        }

        
    }

    public interface IStore
    {
        void Save(int result);
    }

    internal class StringCalculator
    {
        private IStore _store;

        public StringCalculator()
        {
        }

        public StringCalculator(IStore store)
        {
            _store = store;
        }

        internal int Add(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;
            if (!SeparatorExist(input)) return 0;

            var numbers = input.Split(',');
            var total = 0;
            foreach (var number in numbers)
            {
                total += TryparseToInteger(number);
            }

            if (_store!=null)
            {
                if (IsPrime(total))
                {
                    _store.Save(total);
                }
            }

            return total;
        }

        private bool IsPrime(int number)
        {
            if (number==2)
            {
                return true;
            }
            if (number % 2 == 0) return false;
            for (int i = 3; i <= (int)(Math.Sqrt(number)) ; i+=2)
            {
                if (number%i==0)
                {
                    return false;
                }
            }
            return true;
        }

        private int TryparseToInteger(string input)
        {
            int dest;
            if (!int.TryParse(input,out dest))
            {
                throw new ArgumentException("Input format is incorrect");
            }

            return dest;
        }

        private bool SeparatorExist(string input)
        {
            bool isExist = true;

            if (!input.Contains(","))
            {
                isExist = false;
                throw new ArgumentException("Separator is incorrect");
            }

            return isExist;
        }
    }
}
