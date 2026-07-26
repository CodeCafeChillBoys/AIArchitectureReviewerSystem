using System.Linq;
using AIArchitectureReviewer.Domain.Diffing;
using Xunit;

namespace AIArchitectureReviewer.Domain.Tests.Diffing
{
    public class TextDifferTests
    {
        [Fact]
        public void Diff_KhiKhongDoiGi_TraVeToanBoEqual()
        {
            var result = TextDiffer.Diff("a\nb\nc", "a\nb\nc");

            Assert.Equal(3, result.Count);
            Assert.All(result, l => Assert.Equal(DiffOp.Equal, l.Op));
        }

        [Fact]
        public void Diff_ThemDongCuoi_SinhMotInsert()
        {
            var result = TextDiffer.Diff("a\nb", "a\nb\nc");

            var inserts = result.Where(l => l.Op == DiffOp.Insert).ToList();
            Assert.Single(inserts);
            Assert.Equal("c", inserts[0].Text);
            Assert.Equal(3, inserts[0].NewLineNo);
            Assert.Null(inserts[0].OldLineNo);
        }

        [Fact]
        public void Diff_XoaDongGiua_SinhMotDelete()
        {
            var result = TextDiffer.Diff("a\nb\nc", "a\nc");

            var deletes = result.Where(l => l.Op == DiffOp.Delete).ToList();
            Assert.Single(deletes);
            Assert.Equal("b", deletes[0].Text);
            Assert.Equal(2, deletes[0].OldLineNo);
            Assert.Null(deletes[0].NewLineNo);
        }

        [Fact]
        public void Diff_SuaMotDong_SinhMotDeleteVaMotInsert()
        {
            var result = TextDiffer.Diff("a\nb\nc", "a\nB\nc");

            Assert.Single(result.Where(l => l.Op == DiffOp.Delete));
            Assert.Single(result.Where(l => l.Op == DiffOp.Insert));
            Assert.Equal(2, result.Count(l => l.Op == DiffOp.Equal));
        }

        [Fact]
        public void Diff_TuRongSangCoNoiDung_TatCaLaInsert()
        {
            var result = TextDiffer.Diff("", "a\nb");

            Assert.Equal(2, result.Count);
            Assert.All(result, l => Assert.Equal(DiffOp.Insert, l.Op));
        }

        [Fact]
        public void Diff_TuCoNoiDungSangRong_TatCaLaDelete()
        {
            var result = TextDiffer.Diff("a\nb", null);

            Assert.Equal(2, result.Count);
            Assert.All(result, l => Assert.Equal(DiffOp.Delete, l.Op));
        }

        [Fact]
        public void Diff_ChiKhacKieuXuongDong_KhongCoThayDoi()
        {
            var result = TextDiffer.Diff("a\r\nb\r\nc", "a\nb\nc");

            Assert.All(result, l => Assert.Equal(DiffOp.Equal, l.Op));
        }

        [Fact]
        public void Diff_TextKetThucBangXuongDong_KhongSinhDongRongThua()
        {
            var result = TextDiffer.Diff("a\n", "a\n");

            Assert.Single(result);
            Assert.Equal("a", result[0].Text);
        }

        [Fact]
        public void Normalize_DoiCRLFVaCRThanhLF()
        {
            Assert.Equal("a\nb\nc", TextDiffer.Normalize("a\r\nb\rc"));
        }
    }
}
