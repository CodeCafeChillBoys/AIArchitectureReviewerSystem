# Feature Envy

## Mô tả
Feature Envy xảy ra khi một method quan tâm quá nhiều tới dữ liệu hoặc method của object khác hơn là của chính nó. Nói cách khác, hành vi đang đứng sai chỗ: method nhìn “thèm” sang object khác nhiều hơn vào state nội tại của class chứa nó.

Code smell này thường xuất hiện khi logic bị đặt nhầm chỗ, khiến class hiện tại chỉ đóng vai trò gọi chéo sang object khác thay vì xử lý hành vi của chính nó.

## Ý tưởng cốt lõi
- Hành vi nên nằm gần dữ liệu mà nó sử dụng nhiều nhất.
- Nếu một method truy cập nhiều thuộc tính của object khác, có thể logic đó nên được chuyển sang object đó.
- Giảm phụ thuộc chéo giúp code dễ hiểu và dễ bảo trì hơn.

## Dấu hiệu nhận biết
- Một method gọi liên tục getter của object khác.
- Logic của method chủ yếu thao tác trên state của object khác.
- Class hiện tại có rất ít trách nhiệm thực sự.
- Refactor sang move method thường làm code rõ hơn ngay lập tức.

## Tác hại
- Tăng coupling giữa classes.
- Làm class hiện tại trở nên “mỏng” và thiếu ý nghĩa.
- Logic bị đặt sai vị trí làm hệ thống khó mở rộng.
- Dễ vi phạm encapsulation nếu method phải mò sâu vào object khác.

## Nguyên nhân thường gặp
- Thiếu hiểu biết về ownership của hành vi.
- Bắt đầu từ code tạm và không refactor lại.
- Object model chưa được thiết kế rõ về vai trò.
- Dùng getter quá nhiều thay vì để object tự phục vụ chính nó.

## Ví dụ xấu (Python)
```python
class Address:
    def __init__(self, city, country):
        self.city = city
        self.country = country


class Customer:
    def __init__(self, name, address):
        self.name = name
        self.address = address


class ShippingLabelPrinter:
    def print_label(self, customer):
        return f"Ship to {customer.name} in {customer.address.city}, {customer.address.country}"
```

Trong ví dụ này, `print_label` biết quá nhiều về cấu trúc `Customer.address`, trong khi logic định dạng địa chỉ có thể thuộc về `Customer` hoặc `Address`.

## Cách xử lý
- Move Method sang object mà method đang khai thác nhiều nhất.
- Extract Method nếu chỉ cần tách phần formatting/transform nhỏ.
- Introduce Query Method trên object sở hữu dữ liệu.
- Thiết kế lại domain để hành vi nằm gần dữ liệu liên quan.

## Hướng refactor thường dùng
- Move Method: chuyển method sang class có dữ liệu liên quan.
- Preserve Whole Object: truyền cả object thay vì bóc từng field rời rạc.
- Hide Delegate: ẩn việc đi xuyên qua nhiều object.
- Extract Class nếu class hiện tại đang chứa hành vi không đúng vai trò.

## Phân biệt nhanh
- Feature Envy là vấn đề về vị trí của behavior.
- Data Clumps là vấn đề về nhóm dữ liệu lặp lại.
- Inappropriate Intimacy là hai class phụ thuộc nhau quá sâu ở cả hai chiều.

## Checklist
- [ ] Method có truy cập nhiều dữ liệu của object khác hơn của chính nó.
- [ ] Logic có vẻ nên nằm trong class khác.
- [ ] Có thể chuyển method sang object sở hữu dữ liệu đó.
- [ ] Code hiện tại làm lộ cấu trúc nội bộ của object khác.

## Tóm tắt ngắn
Feature Envy là dấu hiệu cho thấy behavior đang đứng sai chỗ. Cách sửa thường hiệu quả nhất là chuyển logic về đúng object sở hữu dữ liệu.

---

Người soạn: ArchReview AI — Feature Envy.