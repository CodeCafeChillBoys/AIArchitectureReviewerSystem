# Adapter Pattern

## Mô tả
Adapter là structural pattern dùng để làm cho hai interface không tương thích có thể làm việc với nhau. Nó hoạt động như một lớp trung gian, chuyển đổi lời gọi từ interface mà client mong đợi sang interface mà đối tượng thật sự hỗ trợ.

Mẫu này đặc biệt hữu ích khi tích hợp thư viện bên ngoài, legacy system, hoặc khi bạn muốn tái sử dụng một class cũ nhưng API của nó không khớp với code hiện tại.

## Ý tưởng cốt lõi
- Chuyển đổi interface, không thay đổi logic nghiệp vụ cốt lõi.
- Che giấu sự khác biệt giữa client và adaptee.
- Giảm nhu cầu sửa code hiện có khi tích hợp component mới.

## Cấu trúc tham gia
- Target: interface mà client mong đợi.
- Adaptee: class hiện có với interface không tương thích.
- Adapter: lớp trung gian chuyển đổi lời gọi.
- Client: chỉ biết Target, không biết Adaptee.

## Khi nào dùng
- Khi cần tích hợp legacy code có method name hoặc tham số khác.
- Khi muốn dùng thư viện bên ngoài nhưng không muốn sửa client.
- Khi cần tái sử dụng class cũ thay vì viết lại từ đầu.

## Khi không nên dùng
- Khi có thể sửa trực tiếp interface của hệ thống mà không gây ảnh hưởng lớn.
- Khi mục tiêu chỉ là đổi tên method nội bộ, không cần lớp trung gian.
- Khi adapter làm cho kiến trúc phức tạp hơn giá trị nó mang lại.

## Ví dụ (Python)
Ví dụ dưới đây chuyển đổi một API cũ sang interface mới mà client mong đợi.

```python
from abc import ABC, abstractmethod


class Target(ABC):
    @abstractmethod
    def request(self):
        pass


class OldApi:
    def specific_request(self):
        return "old response"


class Adapter(Target):
    def __init__(self, adaptee):
        self.adaptee = adaptee

    def request(self):
        return f"Adapter translated -> {self.adaptee.specific_request()}"


client = Adapter(OldApi())
print(client.request())
```

### Giải thích ví dụ
- `OldApi` là `Adaptee`.
- `Target` mô tả interface mới mà client muốn dùng.
- `Adapter` giữ `OldApi` bên trong và chuyển lời gọi `request()` sang `specific_request()`.

## Quy trình hoạt động
1. Client gọi phương thức trên interface Target.
2. Adapter nhận lời gọi đó.
3. Adapter ánh xạ lời gọi sang Adaptee.
4. Kết quả từ Adaptee được trả về theo format mà client hiểu.

## Ưu điểm
- Cho phép tái sử dụng code cũ.
- Tách biệt sự khác nhau giữa các interface.
- Giảm phụ thuộc vào thư viện bên ngoài.
- Dễ thay thế hoặc nâng cấp adaptee hơn.

## Nhược điểm
- Tăng một lớp trung gian trong hệ thống.
- Có thể che giấu sự khác biệt quan trọng giữa hai interface.
- Nếu lạm dụng sẽ tạo ra nhiều lớp mapping khó bảo trì.

## Phân biệt nhanh
- Adapter chuyển interface để code tương thích.
- Facade đơn giản hóa một subsystem, không nhất thiết chuyển đổi interface cũ.
- Decorator thêm hành vi, không tập trung vào tương thích interface.

## Lỗi thường gặp
- Nhầm Adapter với Wrapper đơn thuần và bỏ qua trách nhiệm chuyển đổi interface.
- Đưa toàn bộ logic nghiệp vụ vào adapter thay vì giữ nó mỏng.
- Phụ thuộc vào implementation cụ thể của adaptee thay vì interface.

## Checklist
- [ ] Client chỉ phụ thuộc vào Target.
- [ ] Adapter chỉ chịu trách nhiệm chuyển đổi interface.
- [ ] Adaptee được đóng gói bên trong.
- [ ] Có thể thay đổi nguồn legacy/thư viện mà không đổi client.

## Tóm tắt ngắn
Adapter là lớp cầu nối giúp code mới và code cũ nói chuyện được với nhau mà không cần phá vỡ kiến trúc hiện tại.

---

Người soạn: ArchReview AI — Adapter pattern.