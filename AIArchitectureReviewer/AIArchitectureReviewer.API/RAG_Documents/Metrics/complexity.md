# Complexity

## Mô tả
Complexity đo mức độ khó hiểu, khó theo dõi, và khó dự đoán của code hoặc kiến trúc. Complexity có thể là complexity của thuật toán, control flow, dependency graph, hoặc kiến trúc tổng thể.

Một hệ thống có thể đúng chức năng nhưng vẫn quá phức tạp để bảo trì hiệu quả. Vì vậy, complexity là một metric quan trọng khi đánh giá chất lượng thiết kế.

## Ý tưởng cốt lõi
- Complexity càng cao thì cognitive load càng lớn.
- Không phải complexity nào cũng xấu, nhưng complexity nên có lý do.
- Mục tiêu là giảm complexity không cần thiết, không phải làm mọi thứ “đơn giản hóa quá mức”.

## Các dạng complexity thường gặp
- Cyclomatic complexity: độ phức tạp theo nhánh rẽ trong một hàm.
- Cognitive complexity: độ khó hiểu đối với con người.
- Structural complexity: độ phức tạp của quan hệ giữa các module/class.
- Operational complexity: độ phức tạp khi vận hành hệ thống.

## Dấu hiệu nhận biết
- Hàm có nhiều nhánh `if/elif/else`, loop, hoặc nested logic.
- Code có nhiều exception path hoặc trạng thái chuyển tiếp khó hiểu.
- Module có quá nhiều dependency và boundary không rõ.
- Luồng xử lý phải đọc ngược nhiều chỗ mới hiểu hết.

## Tác hại
- Khó test đầy đủ mọi nhánh.
- Dễ sinh bug ở các path ít được dùng.
- Khó review và khó onboarding.
- Làm refactor tốn thời gian hơn nhiều.

## Nguyên nhân thường gặp
- Logic nghiệp vụ bị nhét vào một nơi quá lớn.
- Thiếu abstraction, nên điều kiện rẽ nhánh tăng dần.
- Tối ưu sớm quá mức.
- Boundary của module/service bị thiết kế mờ.

## Ví dụ xấu (Python)
```python
def calculate_price(order, vip, promo, region, holiday, bulk):
    price = order.base_price
    if vip:
        price *= 0.9
    if promo:
        price *= 0.85
    if region == "EU":
        price *= 1.2
    if holiday:
        price *= 0.95
    if bulk:
        price *= 0.8
    return price
```

Hàm này có nhiều điều kiện độc lập, dễ tăng complexity khi rule tiếp tục mở rộng.

## Cách xử lý
- Tách logic thành các policy hoặc strategy riêng.
- Dùng table-driven design nếu rule theo dữ liệu.
- Chia hàm lớn thành các hàm nhỏ có tên rõ.
- Loại bỏ nhánh không cần thiết và ưu tiên composition.
- Đưa business rule về đúng abstraction thay vì dồn vào một function.

## Hướng refactor thường dùng
- Extract Method.
- Replace Conditional with Polymorphism.
- Strategy.
- Specification pattern hoặc rule engine nếu nhiều rule động.
- Guard clauses để giảm nested depth.

## Phân biệt nhanh
- Complexity là độ khó tổng thể của code hoặc kiến trúc.
- Coupling là mức phụ thuộc giữa thành phần.
- Cohesion là mức tập trung trách nhiệm trong thành phần.

## Checklist
- [ ] Hàm hoặc module có nhiều nhánh rẽ không.
- [ ] Có nested logic sâu không.
- [ ] Có thể tách rule thành phần nhỏ hơn không.
- [ ] Có nhiều dependency hoặc state machine phức tạp không.
- [ ] Complexity tăng có hợp lý với bài toán không.

## Tóm tắt ngắn
Complexity là chỉ báo quan trọng về khả năng hiểu và bảo trì code. Giảm complexity không cần thiết thường là một trong các cách tăng chất lượng hệ thống hiệu quả nhất.

---

Người soạn: ArchReview AI — Complexity.