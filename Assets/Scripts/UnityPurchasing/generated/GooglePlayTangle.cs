// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Qa4lhxnkPRFeEYV04lkHPapjLG/RzboHJXzf4gvTDYJbJ+LBzR2J/En7eFtJdH9wU/8x/450eHh4fHl6BgNwN2TNdpz+aHFxy29aHJ16MfF5Xe3JWSP3JLI83zrA6b31NvKzivNFkG0kusIWBZ6kLMF9bGs5b+pi+3h2eUn7eHN7+3h4eb9qJGxWaArwt02ZW/3pGxs8/6hR/UaPx+kDTr4RNvPwyeCC84mlW/Q2oPuoRZNyKVXNKSqBure81Lc0QAMAUotUaMdL6kiOuMOW3bLj6kxI3Inls8DYZLdYaN0lskx2M9ya8Xz25ZV8poQNDbvMwOHmZaDk7sJUIer+Fx9jDABpxuT2TxKNljeFP9jyaE9Obb84v7rmNm8eJ0BYIHt6eHl4");
        private static int[] order = new int[] { 9,4,9,6,10,8,10,13,9,9,11,13,13,13,14 };
        private static int key = 121;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
