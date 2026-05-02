backend:  
đảm bảo admin account tồn tại  
thêm GET /api/Friendship/suggestion/{userId}/{page} (10 item/page)  
sửa GET /api/Post thêm authorID  
sửa DELETE /api/Post/{postId} để mỗi author xoá được post  
  
frontend:  
thêm friend request, list và suggestion. Có thể add friend bằng cách vào profile user khác -> kết bạn  
thêm tính năng sửa profile (bao gồm fullName và bio)  
thêm visibility cho post (chưa hoạt động)  
thêm story và post của user đã đăng ở profile  
thêm search user  

thiếu:  
like post (FE/BE)  
đăng story (FE)  
comment (FE)  
report (FE)  
friendship: block, reject, remove friend (FE)  
chức năng liên quan đến media đang lỗi (do không kết nối azure blob) (FE?/BE?)  

known bug:  
user không có role user   

