# Maintainability

## Mô tả
Maintainability là khả năng của hệ thống trong việc được hiểu, sửa, kiểm thử, mở rộng, và vận hành với chi phí hợp lý trong suốt vòng đời. Đây không phải một metric đơn lẻ mà là tổng hợp của nhiều yếu tố như coupling, cohesion, complexity, clarity, testability, và consistency.

Nói ngắn gọn, maintainability trả lời câu hỏi: khi hệ thống thay đổi, chúng ta có thể sửa nó nhanh và an toàn đến mức nào?

## Ý tưởng cốt lõi
- Hệ thống dễ bảo trì là hệ thống dễ hiểu và dễ thay đổi.
- Maintainability phụ thuộc vào cả code, kiến trúc, testing, tooling, và quy trình.
- Không chỉ nhìn vào hôm nay; phải nhìn cả chi phí thay đổi về sau.

## Các yếu tố cấu thành
- Readability: code dễ đọc và dễ hiểu.
- Modularity: thành phần được tách rõ.
- Testability: kiểm thử dễ và đáng tin cậy.
- Changeability: thay đổi nhỏ không gây ripple effect lớn.
- Consistency: quy ước và cấu trúc thống nhất.
- Observability: dễ theo dõi hành vi khi chạy.

## Dấu hiệu của maintainability tốt
- Cấu trúc thư mục và module rõ ràng.
- Hàm/class có trách nhiệm đơn nhất.
- Test có thể chạy ổn định và nhanh.
- Dependency được quản lý tốt.
- Có chuẩn code và naming nhất quán.

## Dấu hiệu của maintainability kém
- Sửa một chỗ kéo theo nhiều chỗ.
- Code khó đọc, naming mơ hồ, logic rối.
- Test flakey hoặc quá khó viết.
- Boundary kiến trúc mờ, coupling cao.
- Tài liệu và code không khớp nhau.

## Tác hại khi maintainability thấp
- Tốc độ phát triển giảm dần theo thời gian.
- Bug fix trở nên đắt hơn.
- Onboarding người mới khó.
- Team sợ refactor.
- Nợ kỹ thuật tích lũy ngày càng lớn.

## Cách cải thiện
- Giảm coupling và tăng cohesion.
- Giữ complexity ở mức hợp lý.
- Viết test cho business logic và boundary quan trọng.
- Chuẩn hóa naming, module structure, và style.
- Refactor dần dần thay vì để debt tích tụ.
- Dùng architecture phù hợp với quy mô hệ thống.

## Ví dụ thực tế
Một service có thể vẫn chạy đúng nhưng rất khó maintain nếu:
- class lớn, nhiều dependency,
- rule nghiệp vụ rải rác,
- test chậm và khó mock,
- mỗi thay đổi đều chạm nhiều file,
- và không ai còn dám đụng vào vì sợ vỡ.

Ngược lại, một hệ thống dễ maintain thường có boundary rõ, class nhỏ, test tốt, và luồng thay đổi dễ dự đoán.

## Mối liên hệ với các metric khác
- Coupling cao thường làm maintainability giảm.
- Cohesion cao thường làm maintainability tăng.
- Complexity cao thường làm maintainability giảm.
- Good testing và observability cải thiện maintainability đáng kể.

## Checklist
- [ ] Code có dễ đọc không.
- [ ] Module/class có trách nhiệm rõ không.
- [ ] Thay đổi nhỏ có lan rộng quá nhiều không.
- [ ] Test có đủ nhanh và ổn định không.
- [ ] Team có thể refactor mà không sợ làm hỏng hệ thống không.
- [ ] Quy ước code và kiến trúc có nhất quán không.

## Tóm tắt ngắn
Maintainability là mục tiêu tổng hợp của thiết kế tốt: dễ hiểu, dễ sửa, dễ test, và ít rủi ro khi thay đổi. Nó được xây dựng từ việc quản lý tốt coupling, cohesion, và complexity.

---

Người soạn: ArchReview AI — Maintainability.