using AIArchitectureReviewer.Domain.Diffing;
using Xunit;

namespace AIArchitectureReviewer.Domain.Tests.Diffing
{
    public class TextDifferHunkTests
    {
        [Fact]
        public void ToHunks_KhiKhongDoiGi_TraVeRong()
        {
            var lines = TextDiffer.Diff("a\nb\nc", "a\nb\nc");

            Assert.Empty(TextDiffer.ToHunks(lines));
        }

        [Fact]
        public void ToHunks_SuaMotDong_TraVeMotHunkCoDongNguCanh()
        {
            var oldText = "1\n2\n3\n4\n5";
            var newText = "1\n2\nX\n4\n5";

            var hunks = TextDiffer.ToHunks(TextDiffer.Diff(oldText, newText));

            Assert.Single(hunks);
            Assert.Equal(1, hunks[0].OldStart);
            Assert.Equal(5, hunks[0].OldCount);
            Assert.Equal(1, hunks[0].NewStart);
            Assert.Equal(5, hunks[0].NewCount);
        }

        [Fact]
        public void ToHunks_HaiDoanSuaCachXaNhau_TraVeHaiHunk()
        {
            var oldText = "1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15";
            var newText = "X\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\nY";

            var hunks = TextDiffer.ToHunks(TextDiffer.Diff(oldText, newText));

            Assert.Equal(2, hunks.Count);
        }

        [Fact]
        public void ToHunks_HaiDoanSuaGanNhau_GopThanhMotHunk()
        {
            var oldText = "1\n2\n3\n4\n5\n6";
            var newText = "X\n2\n3\n4\n5\nY";

            var hunks = TextDiffer.ToHunks(TextDiffer.Diff(oldText, newText));

            Assert.Single(hunks);
        }

        [Fact]
        public void ToUnifiedDiff_SinhDungTienToVaHeader()
        {
            var hunks = TextDiffer.ToHunks(TextDiffer.Diff("a\nb\nc", "a\nB\nc"));

            var text = TextDiffer.ToUnifiedDiff(hunks);

            Assert.Contains("@@ -1,3 +1,3 @@", text);
            Assert.Contains("\n-b\n", text);
            Assert.Contains("\n+B\n", text);
            Assert.Contains(" a\n", text);
        }

        [Fact]
        public void ToUnifiedDiff_KhiKhongCoHunk_TraVeChuoiRong()
        {
            Assert.Equal(string.Empty, TextDiffer.ToUnifiedDiff(TextDiffer.ToHunks(TextDiffer.Diff("a", "a"))));
        }

        [Fact]
        public void ToHunks_ChiToanInsert_OldStartVaOldCountBangKhong()
        {
            var hunks = TextDiffer.ToHunks(TextDiffer.Diff("", "a\nb"));

            Assert.Single(hunks);
            Assert.Equal(0, hunks[0].OldStart);
            Assert.Equal(0, hunks[0].OldCount);
            Assert.Equal(1, hunks[0].NewStart);
            Assert.Equal(2, hunks[0].NewCount);
        }
    }
}
