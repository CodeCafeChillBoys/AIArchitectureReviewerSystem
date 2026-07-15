# Cyclic Dependency

## Mô tả
Cyclic Dependency xảy ra khi hai hoặc nhiều module, package, hoặc class phụ thuộc lẫn nhau theo vòng khép kín. Ví dụ A phụ thuộc B, B phụ thuộc C, và C lại phụ thuộc ngược về A.

Vòng phụ thuộc làm kiến trúc cứng hơn, khó test hơn, và rất dễ gây ripple effect khi thay đổi một phần của hệ thống.

## Ý tưởng cốt lõi
- Phụ thuộc nên có hướng rõ ràng và tránh vòng lặp.
- Module cấp cao không nên bị kéo xuống bởi module cấp thấp một cách vòng tròn.
- Tách interface và đảo chiều dependency có thể phá vòng lặp.

## Dấu hiệu nhận biết
- Import lẫn nhau giữa modules.
- Package cấu hình khó vì thứ tự khởi tạo phụ thuộc vòng tròn.
- Khi mở rộng một module, nhiều module khác bị ảnh hưởng ngược lại.
- Test hoặc build gặp lỗi circular import / circular reference.

## Tác hại
- Khó hiểu luồng phụ thuộc.
- Khó test từng module độc lập.
- Dễ gây lỗi khởi tạo và lỗi import.
- Cản trở việc tái sử dụng hoặc tách module.

## Nguyên nhân thường gặp
- Thiết kế module không có boundary rõ ràng.
- Cho class biết quá nhiều về nhau.
- Dùng object/reference trực tiếp thay vì interface.
- Đặt logic chung sai chỗ khiến module phải gọi chéo nhau.

## Ví dụ xấu (Python)
```python
class A:
    def __init__(self, b):
        self.b = b

    def run(self):
        return self.b.help()


class B:
    def __init__(self, a=None):
        self.a = a

    def help(self):
        return "help from B"

    def call_a(self):
        return self.a.run()
```

Khi A và B cần gọi nhau hai chiều, quan hệ phụ thuộc bắt đầu bị vòng tròn. Trong hệ thống lớn, kiểu phụ thuộc này thường biểu hiện ở mức module hoặc package.

## Cách xử lý
- Đảo phụ thuộc qua interface hoặc abstract class.
- Tách shared logic ra module trung gian.
- Dùng event/callback thay vì gọi trực tiếp hai chiều.
- Áp dụng dependency injection để chỉ phụ thuộc một chiều.
- Chia lại boundary theo domain hoặc layer.

## Hướng refactor thường dùng
- Extract Interface.
- Dependency Inversion.
- Move Class hoặc Move Method.
- Introduce Mediator hoặc Facade nếu đang có gọi chéo phức tạp.
- Split Package / Split Module nếu boundary quá mờ.

## Phân biệt nhanh
- Cyclic Dependency là vấn đề cấu trúc phụ thuộc vòng tròn.
- Shotgun Surgery là vấn đề thay đổi nhỏ phải chạm nhiều nơi.
- Inappropriate Intimacy là hai class quá phụ thuộc chi tiết của nhau.

## Checklist
- [ ] Có vòng phụ thuộc giữa classes, modules, hoặc packages không.
- [ ] Có import/call chéo hai chiều không.
- [ ] Có thể đảo dependency bằng interface không.
- [ ] Có thể tách shared logic ra module trung gian không.

## Tóm tắt ngắn
Cyclic Dependency làm kiến trúc khó kiểm soát và khó test. Cần phá vòng bằng interface, tách module, hoặc đảo chiều dependency.

---

Người soạn: ArchReview AI — Cyclic Dependency.