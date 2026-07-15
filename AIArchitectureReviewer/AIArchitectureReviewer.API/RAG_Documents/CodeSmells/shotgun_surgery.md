# Shotgun Surgery

## Mô tả
Shotgun Surgery xảy ra khi một thay đổi nhỏ ở một nơi lại đòi hỏi phải sửa nhiều file, nhiều class, hoặc nhiều method ở khắp hệ thống. Nói cách khác, một thay đổi đơn giản buộc phải “bắn” ra rất nhiều vị trí khác nhau.

Code smell này thường phản ánh việc responsibility bị phân tán sai, khiến một khái niệm nghiệp vụ không có “nhà” rõ ràng.

## Ý tưởng cốt lõi
- Một thay đổi nên chạm vào ít nơi nhất có thể.
- Nếu cùng một lý do thay đổi xuất hiện ở nhiều vị trí, logic đang bị phân mảnh.
- Tập trung các thay đổi liên quan về đúng abstraction sẽ làm hệ thống bền hơn.

## Dấu hiệu nhận biết
- Sửa một rule đơn giản nhưng phải cập nhật nhiều file.
- Một thay đổi yêu cầu đồng bộ nhiều class nhỏ.
- Cùng một logic được copy nhiều nơi nên phải sửa hàng loạt.
- Change set thường lớn hơn bản chất nghiệp vụ.

## Tác hại
- Chi phí bảo trì cao.
- Rủi ro bỏ sót một vị trí khi sửa.
- Dễ sinh bug do không đồng bộ.
- Làm việc nhóm khó hơn vì nhiều người đụng vào nhiều nơi cho cùng một change.

## Nguyên nhân thường gặp
- Logic bị rải rác qua nhiều class không có owner rõ ràng.
- Copy/paste code hoặc cấu trúc dữ liệu.
- Không có abstraction trung tâm cho một nghiệp vụ.
- Kiến trúc module lỏng lẻo, boundary không rõ.

## Ví dụ xấu (Python)
```python
# Khi đổi định dạng thuế, phải sửa rất nhiều nơi khác nhau
class InvoicePrinter:
    def print_total(self, amount):
        return amount * 1.1

class ReportService:
    def summary(self, amount):
        return amount * 1.1

class PaymentService:
    def charge(self, amount):
        return amount * 1.1
```

Nếu thuế thay đổi, cùng một logic phải sửa ở nhiều class. Đây là dấu hiệu mạnh của Shotgun Surgery.

## Cách xử lý
- Tập trung logic thay đổi vào một abstraction hoặc service chung.
- Dùng domain object hoặc policy object cho rule thay đổi.
- Tách các thay đổi theo feature hoặc bounded context rõ ràng.
- Nếu logic lặp lại, kết hợp với refactor loại bỏ duplicated code.

## Hướng refactor thường dùng
- Move Method hoặc Move Field về nơi sở hữu khái niệm.
- Extract Class để gom logic thay đổi cùng nhau.
- Introduce Parameter Object hoặc Value Object cho rule liên quan.
- Replace Conditional with Polymorphism khi thay đổi theo loại.

## Phân biệt nhanh
- Shotgun Surgery là một change kéo theo nhiều file phải sửa.
- Duplicated Code là nội dung lặp lại ở nhiều nơi, thường là nguyên nhân của Shotgun Surgery.
- Divergent Change là một class hay module phải thay đổi vì quá nhiều lý do khác nhau.

## Checklist
- [ ] Một thay đổi nhỏ có khiến nhiều file phải sửa không.
- [ ] Cùng một rule nằm rải rác ở nhiều nơi không.
- [ ] Có abstraction chung để gom các thay đổi cùng loại không.
- [ ] Việc sửa có dễ bỏ sót một vị trí không.

## Tóm tắt ngắn
Shotgun Surgery cho thấy thay đổi đang bị phân tán quá mức. Cần gom logic liên quan về cùng một abstraction để giảm số nơi phải sửa.

---

Người soạn: ArchReview AI — Shotgun Surgery.