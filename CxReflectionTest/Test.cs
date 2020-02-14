using System;

namespace CxSolution.CxReflectionTest
{
    public class TestBase
    {
        public static int int_static=5;
        public event Action base_public_Action_event;

        private event Action base_private_Action_event;

        public Action base_public_Action;

        private Action base_private_Action;

        public string base_public_pro_int
        {
            get;
            set;
        }
        private string base_private_pro_int
        {
            get;
            set;
        }

        public int base_public_int;

        private int base_private_int;
        public int base_Add(int left, int right)
        {
            return left + right;
        }
        private int base_Cut(int left, int right)
        {
            return left - right;
        }
    }
    public class Test : TestBase
    {
        public delegate void testEvent();

        public event testEvent public_event;

        private event testEvent private_event;
        public string public_pro_int
        {
            get;
            set;
        }
        private string private_pro_int
        {
            get;
            set;
        }

        public int public_int;

        private int private_int;

        public int Add(int left, int right)
        {
            return left + right;
        }
        private int Cut(int left, int right)
        {
            return left - right;
        }
    }
}
