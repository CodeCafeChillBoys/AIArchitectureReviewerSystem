# Data Clumps

## Mô tả
Data Clumps xảy ra khi cùng một nhóm dữ liệu xuất hiện lặp đi lặp lại ở nhiều nơi, thường dưới dạng một cụm tham số, một cụm field, hoặc một cụm object luôn đi cùng nhau.

Nếu bạn thấy nhiều method cùng nhận `city`, `street`, `zip_code`, hoặc nhiều class cùng chứa một bộ thuộc tính gần như giống nhau, đó là dấu hiệu của Data Clumps.

## Ý tưởng cốt lõi
- Dữ liệu đi cùng nhau nhiều lần thì nên được gom lại thành một abstraction.
- Cụm dữ liệu lặp lại thường báo hiệu thiếu domain object hoặc value object.
- Khi các tham số luôn xuất hiện cùng nhau, có thể chúng đang mô tả cùng một khái niệm.

## Dấu hiệu nhận biết
- Nhiều method nhận cùng một nhóm tham số giống nhau.
- Nhiều class có bộ field gần giống nhau.
- Code truyền qua lại nhiều primitive values thay vì object có ý nghĩa.
- Một nhóm dữ liệu luôn phải nhớ theo cùng một thứ tự.

## Tác hại
- Khó đọc vì phải nhớ nhiều tham số cùng lúc.
- Dễ truyền nhầm thứ tự hoặc thiếu tham số.
- Thay đổi schema dữ liệu phải sửa nhiều nơi.
- Logic liên quan bị phân tán thay vì gom lại.

## Nguyên nhân thường gặp
- Thiếu value object hoặc domain object phù hợp.
- Thiết kế ban đầu vội vàng, chưa nhận diện khái niệm nghiệp vụ.
- Copy/paste API giữa các lớp hoặc service.

## Ví dụ xấu (Python)
```python
class ShippingService:
    def create_label(self, street, city, zip_code, country):
        return f"{street}, {city}, {zip_code}, {country}"

    def validate_address(self, street, city, zip_code, country):
        return all([street, city, zip_code, country])
```

Bốn tham số này luôn đi cùng nhau, cho thấy chúng nên được gom thành một object `Address`.

## Cách xử lý
- Introduce Parameter Object hoặc Value Object.
- Group các thuộc tính liên quan vào một class có tên rõ nghĩa.
- Đưa validation và behavior liên quan vào object mới.
- Dùng builder hoặc factory nếu object có nhiều tùy chọn tạo.

## Hướng refactor thường dùng
- Introduce Parameter Object: gom nhiều tham số thành một object.
- Preserve Whole Object: truyền object thay vì bóc field.
- Extract Class: nếu một nhóm field lặp lại ở nhiều nơi.
- Move Behavior Into Object nếu validation/formatting đi cùng cụm dữ liệu.

## Phân biệt nhanh
- Data Clumps là cụm dữ liệu lặp lại.
- Primitive Obsession là lạm dụng kiểu dữ liệu đơn giản cho khái niệm phức tạp.
- Duplicate Code là logic lặp lại, không chỉ dữ liệu lặp.

## Checklist
- [ ] Có một nhóm tham số luôn xuất hiện cùng nhau.
- [ ] Nhiều class trùng cụm field tương tự.
- [ ] Dữ liệu có thể được gom thành value object.
- [ ] Validation hoặc behavior liên quan nên đặt gần data hơn.

## Tóm tắt ngắn
Data Clumps cho thấy hệ thống đang truyền các cụm dữ liệu như primitive rời rạc. Cách chữa tốt nhất là tạo abstraction có nghĩa thay cho một loạt tham số rời.

---

Người soạn: ArchReview AI — Data Clumps.