using System.Linq;
using AIArchitectureReviewer.Domain.Diffing;
using Xunit;

namespace AIArchitectureReviewer.Domain.Tests.Diffing
{
    public class ChangeSetBuilderTests
    {
        [Fact]
        public void Scalar_KhiGiaTriMoiLaNull_BoQuaViClientKhongGui()
        {
            var changeSet = new ChangeSetBuilder()
                .Scalar("Name", "cũ", null)
                .Build();

            Assert.False(changeSet.HasChanges);
        }

        [Fact]
        public void Scalar_KhiGiaTriGiongNhau_KhongGhiNhanThayDoi()
        {
            var changeSet = new ChangeSetBuilder()
                .Scalar("Name", "giống", "giống")
                .Build();

            Assert.False(changeSet.HasChanges);
        }

        [Fact]
        public void Scalar_KhiGiaTriKhacNhau_GhiNhanOldVaNew()
        {
            var changeSet = new ChangeSetBuilder()
                .Scalar("Name", "cũ", "mới")
                .Build();

            Assert.True(changeSet.HasChanges);
            var change = Assert.Single(changeSet.Changes);
            Assert.Equal("Name", change.Field);
            Assert.Equal("cũ", change.OldValue);
            Assert.Equal("mới", change.NewValue);
            Assert.Null(change.UnifiedDiff);
        }

        [Fact]
        public void Text_KhiChiKhacKieuXuongDong_KhongGhiNhanThayDoi()
        {
            var changeSet = new ChangeSetBuilder()
                .Text("Content", "a\r\nb", "a\nb")
                .Build();

            Assert.False(changeSet.HasChanges);
        }

        [Fact]
        public void Text_KhiNoiDungDoi_GhiUnifiedDiffVaDemDongThemBot()
        {
            var changeSet = new ChangeSetBuilder()
                .Text("Content", "a\nb\nc", "a\nB\nc\nd")
                .Build();

            var change = Assert.Single(changeSet.Changes);
            Assert.Equal("Content", change.Field);
            Assert.Null(change.OldValue);
            Assert.Null(change.NewValue);
            Assert.NotNull(change.UnifiedDiff);
            Assert.Contains("@@", change.UnifiedDiff);
            Assert.Equal(2, change.Additions);
            Assert.Equal(1, change.Deletions);
        }

        [Fact]
        public void Contains_TraVeDungTheoTenField()
        {
            var changeSet = new ChangeSetBuilder()
                .Scalar("Name", "cũ", "mới")
                .Scalar("DiagramType", "UML", "UML")
                .Build();

            Assert.True(changeSet.Contains("Name"));
            Assert.False(changeSet.Contains("DiagramType"));
        }

        [Fact]
        public void Build_MoiLanGoiSinhIdRieng()
        {
            var a = new ChangeSetBuilder().Scalar("Name", "x", "y").Build();
            var b = new ChangeSetBuilder().Scalar("Name", "x", "y").Build();

            Assert.NotEqual(a.Id, b.Id);
        }

        [Fact]
        public void Builder_GiuNguyenThuTuFieldDuocKhaiBao()
        {
            var changeSet = new ChangeSetBuilder()
                .Scalar("A", "1", "2")
                .Text("B", "x", "y")
                .Scalar("C", "3", "4")
                .Build();

            Assert.Equal(new[] { "A", "B", "C" }, changeSet.Changes.Select(c => c.Field));
        }

        [Fact]
        public void Text_KhiCurrentLaNullVaIncomingLaChuoiRong_CoYKhongGhiNhanThayDoi()
        {
            // Với field text dài, "chưa có nội dung" và "nội dung rỗng" là một.
            var changeSet = new ChangeSetBuilder()
                .Text("Content", null, "")
                .Build();

            Assert.False(changeSet.HasChanges);
        }

        [Fact]
        public void Text_KhiCurrentCoNoiDungVaIncomingLaChuoiRong_VanGhiNhanThayDoi()
        {
            // Xoá sạch nội dung vẫn phải là một thay đổi có thật.
            var changeSet = new ChangeSetBuilder()
                .Text("Content", "a\nb", "")
                .Build();

            var change = Assert.Single(changeSet.Changes);
            Assert.Equal("Content", change.Field);
            Assert.Equal(2, change.Deletions);
            Assert.Equal(0, change.Additions);
        }

        [Fact]
        public void Text_KhiChiKhacKyTuXuongDongCuoi_KhongGhiNhanThayDoi()
        {
            var changeSet = new ChangeSetBuilder()
                .Text("Content", "line1\nline2\n", "line1\nline2")
                .Build();

            Assert.False(changeSet.HasChanges);
        }

        [Fact]
        public void Text_KhiKhacCaKieuXuongDongVaXuongDongCuoi_KhongGhiNhanThayDoi()
        {
            var changeSet = new ChangeSetBuilder()
                .Text("Content", "line1\r\nline2\r\n", "line1\nline2")
                .Build();

            Assert.False(changeSet.HasChanges);
        }
    }
}
