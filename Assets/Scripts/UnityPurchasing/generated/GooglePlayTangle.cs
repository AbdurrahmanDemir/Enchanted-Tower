// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("8OybJgRd/sMq8iyjegbD4Ow8qN1YfMzoeALWBZMd/hvhyJzUF9OSq58wF9LR6MGj0qiEetUXgdqJZLJT2llXWGjaWVJa2llZWJ5LBU13SStI58XXbjOstxakHvnTSW5vTJ4ZntGWbLh63Mg6Oh3eiXDcZ67myCJvlnlJ/ASTbVcS/bvQXdfEtF2HpSwIdOwIC6Cblp31lhVhIiFzqnVJ5mCPBKY4xRwwfzCkVcN4JhyLQg1Oastpr5nit/yTwsttaf2oxJLh+UUnIlEWRexXvd9JUFDqTns9vFsQ0GjaWXpoVV5Rct4Q3q9VWVlZXVhb0mSxTAWb4zckv4UN4FxNShhOy0Msmu3hwMdEgcXP43UAy982PkItIZvHF04/BmF5AVpbWVhZ");
        private static int[] order = new int[] { 4,10,2,10,12,13,7,13,9,11,13,12,12,13,14 };
        private static int key = 88;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
