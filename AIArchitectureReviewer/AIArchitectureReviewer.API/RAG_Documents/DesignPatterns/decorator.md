# Decorator Pattern

## Mô tả
Decorator là structural pattern cho phép thêm hành vi cho một object một cách động bằng cách bọc object đó trong một lớp decorator. Thay vì kế thừa để mở rộng, decorator cho phép kết hợp nhiều lớp bọc để thêm nhiều tính năng độc lập.

## Ý tưởng cốt lõi
- Bọc object bằng một lớp khác có cùng interface.
- Tăng hoặc thay đổi hành vi mà không sửa lớp gốc.
- Cho phép chồng nhiều decorator với nhau.

## Cấu trúc tham gia
- Component: interface chung.
- Concrete Component: object gốc.
- Decorator: giữ tham chiếu đến component và triển khai cùng interface.
- Concrete Decorator: thêm hành vi cụ thể.

## Khi nào dùng
- Khi muốn thêm trách nhiệm cho object tại runtime.
- Khi subclassing sẽ tạo quá nhiều lớp con.
- Khi muốn kết hợp nhiều tính năng theo kiểu linh hoạt.

## Khi không nên dùng
- Khi chỉ cần vài biến thể cố định và subclass là đủ.
- Khi decorator làm luồng gọi trở nên khó đọc hoặc khó debug.

## Ví dụ (Python)
Ví dụ dưới đây thêm logging và formatting cho một text renderer.

```python
from abc import ABC, abstractmethod


class TextComponent(ABC):
    @abstractmethod
    def render(self):
        pass


class PlainText(TextComponent):
    def __init__(self, text):
        self.text = text

    def render(self):
        return self.text


class TextDecorator(TextComponent):
    def __init__(self, component):
        self.component = component


class BoldDecorator(TextDecorator):
    def render(self):
        return f"<b>{self.component.render()}</b>"


class ItalicDecorator(TextDecorator):
    def render(self):
        return f"<i>{self.component.render()}</i>"


class LoggingDecorator(TextDecorator):
    def render(self):
        result = self.component.render()
        return f"[logged] {result}"


text = PlainText("Hello")
formatted = LoggingDecorator(BoldDecorator(ItalicDecorator(text)))
print(formatted.render())
```

### Giải thích ví dụ
- `PlainText` là concrete component.
- `TextDecorator` giữ component bên trong.
- `BoldDecorator`, `ItalicDecorator`, `LoggingDecorator` là concrete decorators.
- Các decorator có thể xếp chồng theo nhiều thứ tự khác nhau.

## Quy trình hoạt động
1. Client tạo component gốc.
2. Client bọc component bằng một hoặc nhiều decorator.
3. Khi gọi method, decorator xử lý thêm logic của nó.
4. Decorator có thể gọi tiếp xuống component bên trong.

## Ưu điểm
- Mở rộng hành vi mà không cần subclass.
- Kết hợp tính năng linh hoạt theo runtime.
- Nguyên tắc đơn trách nhiệm rõ hơn so với class phình to.

## Nhược điểm
- Nhiều lớp bọc có thể làm luồng chạy khó theo dõi.
- Debug có thể khó hơn vì hành vi bị chia nhỏ.
- Thứ tự decorator có thể ảnh hưởng kết quả.

## Phân biệt nhanh
- Decorator thêm chức năng.
- Adapter đổi interface.
- Facade đơn giản hóa subsystem.

## Lỗi thường gặp
- Quên gọi component bên trong khi cần chain hành vi.
- Đưa logic nghiệp vụ vào decorator thay vì chỉ thêm tính năng phụ trợ.
- Chồng decorator mà không kiểm soát thứ tự áp dụng.

## Checklist
- [ ] Decorator và component cùng interface.
- [ ] Có thể bọc nhiều lớp mà vẫn hoạt động.
- [ ] Logic thêm vào là độc lập và có thể tách rời.
- [ ] Không cần sửa lớp gốc để thêm hành vi.

## Tóm tắt ngắn
Decorator cho phép thêm hành vi động bằng cách bọc object, rất phù hợp khi cần nhiều biến thể kết hợp mà không muốn nổ số lượng subclass.

---

Người soạn: ArchReview AI — Decorator pattern.