# Abstract Factory Pattern

## Mô tả
sAbstract Factory là một creational pattern dùng để tạo ra các họ đối tượng có liên quan với nhau mà không phụ thuộc vào lớp cụ thể của từng object.

Điểm quan trọng nhất của pattern này là client không khởi tạo trực tiếp từng concrete class. Thay vào đó, client chỉ làm việc với một interface factory trừu tượng, còn factory cụ thể sẽ quyết định sẽ tạo ra bộ sản phẩm nào.

Mẫu này thường xuất hiện khi ứng dụng cần hỗ trợ nhiều “theme”, “platform”, “runtime environment” hoặc “product family” khác nhau. Ví dụ: bộ widget Windows, bộ widget macOS, hoặc bộ UI Light/Dark.

## Ý tưởng cốt lõi
- Tạo ra nhiều object nhưng giữ tính nhất quán giữa chúng.
- Gom logic khởi tạo của cả một họ sản phẩm vào một chỗ.
- Cho phép thay thế toàn bộ họ sản phẩm chỉ bằng cách đổi factory.
- Giảm phụ thuộc giữa client và concrete classes.

## Cấu trúc tham gia
- Abstract Factory: khai báo các phương thức tạo sản phẩm.
- Concrete Factory: triển khai cách tạo từng sản phẩm cụ thể trong cùng một họ.
- Abstract Product: interface/base class cho từng loại sản phẩm.
- Concrete Product: các lớp cụ thể được factory tạo ra.
- Client: chỉ gọi qua interface của factory và product.

## Khi nào dùng
- Khi hệ thống phải tạo ra nhiều object có quan hệ chặt chẽ với nhau.
- Khi muốn đảm bảo các object được tạo ra luôn “đi cùng họ” và tương thích với nhau.
- Khi muốn thay đổi toàn bộ bộ sản phẩm mà không sửa client.
- Khi cần tách phần tạo object khỏi phần sử dụng object.

## Khi không nên dùng
- Khi chỉ có một vài object đơn giản và không có nhóm sản phẩm liên quan.
- Khi việc thêm factory và interface chỉ làm code phức tạp hơn so với lợi ích.
- Khi chỉ cần thay đổi một loại object duy nhất, Factory Method có thể đủ.

## Ví dụ (Python)
Ví dụ dưới đây mô tả bộ giao diện GUI theo từng nền tảng. Mỗi factory sẽ tạo button và checkbox cùng “họ” với nhau.

```python
from abc import ABC, abstractmethod


class Button(ABC):
    @abstractmethod
    def paint(self):
        pass


class Checkbox(ABC):
    @abstractmethod
    def paint(self):
        pass


class WindowsButton(Button):
    def paint(self):
        return "Render Windows button"


class MacButton(Button):
    def paint(self):
        return "Render Mac button"


class WindowsCheckbox(Checkbox):
    def paint(self):
        return "Render Windows checkbox"


class MacCheckbox(Checkbox):
    def paint(self):
        return "Render Mac checkbox"


class GUIFactory(ABC):
    @abstractmethod
    def create_button(self):
        pass

    @abstractmethod
    def create_checkbox(self):
        pass


class WindowsFactory(GUIFactory):
    def create_button(self):
        return WindowsButton()

    def create_checkbox(self):
        return WindowsCheckbox()


class MacFactory(GUIFactory):
    def create_button(self):
        return MacButton()

    def create_checkbox(self):
        return MacCheckbox()


class Application:
    def __init__(self, factory):
        self.factory = factory

    def render(self):
        button = self.factory.create_button()
        checkbox = self.factory.create_checkbox()
        return [button.paint(), checkbox.paint()]


app_windows = Application(WindowsFactory())
app_mac = Application(MacFactory())

print(app_windows.render())
print(app_mac.render())
```

### Giải thích ví dụ
- `GUIFactory` là interface tạo họ sản phẩm.
- `WindowsFactory` và `MacFactory` là các factory cụ thể.
- `Button` và `Checkbox` là product abstractions.
- `WindowsButton`, `MacButton`, `WindowsCheckbox`, `MacCheckbox` là concrete products.
- `Application` là client, chỉ biết gọi factory để lấy đối tượng.

## Quy trình hoạt động
1. Client chọn một concrete factory phù hợp với môi trường.
2. Client gọi các phương thức tạo product qua abstract factory.
3. Factory tạo ra các concrete product tương thích với nhau.
4. Client sử dụng các product qua abstraction, không cần biết lớp cụ thể.

## Ưu điểm
- Tách biệt logic khởi tạo khỏi logic sử dụng.
- Dễ thay đổi toàn bộ họ sản phẩm.
- Giảm phụ thuộc vào concrete class.
- Đảm bảo các sản phẩm được tạo ra đồng bộ theo từng họ.

## Nhược điểm
- Tăng số lượng interface và class.
- Khó mở rộng nếu muốn thêm một loại product mới cho tất cả factory.
- Code có thể phức tạp hơn so với Factory Method hoặc khởi tạo trực tiếp.

## Phân biệt nhanh với Factory Method
- Factory Method thường tập trung vào việc tạo một loại object.
- Abstract Factory tập trung vào việc tạo cả một họ object liên quan.
- Factory Method thường là một method; Abstract Factory là một bộ interface gồm nhiều method tạo sản phẩm.

## Lỗi thường gặp
- Trộn các product không cùng họ, ví dụ button của Windows nhưng checkbox của Mac.
- Dùng Abstract Factory cho bài toán quá nhỏ, khiến code bị cồng kềnh.
- Để client biết concrete class, làm mất ý nghĩa của abstraction.
- Không thiết kế trước danh sách product cần tạo, dẫn đến khó mở rộng về sau.

## Checklist
- [ ] Có nhiều loại sản phẩm cần tạo thành từng họ nhất quán.
- [ ] Client chỉ làm việc với abstraction.
- [ ] Concrete factory quyết định family của product.
- [ ] Không khởi tạo concrete class trực tiếp trong client.
- [ ] Có thể thay đổi họ sản phẩm mà ít ảnh hưởng đến client.

## Tóm tắt ngắn
Abstract Factory phù hợp khi bạn cần tạo nhiều object có liên quan và muốn thay đổi toàn bộ bộ object đó một cách an toàn, đồng nhất, và ít phụ thuộc vào concrete implementation.

---

Người soạn: ArchReview AI — Abstract Factory pattern.
