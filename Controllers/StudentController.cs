using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StudentManager.DataRepository;
using StudentManager.Models;
using StudentManager.ViewModels;

namespace StudentManager.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, 
            ILogger<StudentController> logger)
        {
            _studentRepository = studentRepository;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            //此处实际类型类IEnumerable<Student>
            var students = _studentRepository.GetAllStduents();
            return View(students);
        }

        public IActionResult Details(int id)
        {
            _logger.LogTrace("Trace(跟踪) Log");
            _logger.LogDebug("Debug(调试) Log");
            _logger.LogInformation("信息(Information) Log");
            _logger.LogWarning("警告(Warning) Log");
            _logger.LogError("错误(Error) Log");
            _logger.LogCritical("严重(Critical) Log");
            //如何对SQL也进行跟踪？  在SQL操作的服务类中也添加上述操作

            Student student = _studentRepository.GetStudent(id);

            if(student == null)
            {
                Response.StatusCode = 404;
                return View("StudentNotFound", id);
            }
            //ViewModel的引入
            StudentDetailsViewModel studentDetailsViewModel = new StudentDetailsViewModel
            {
                Student = student,
                PageTitle = "学生详细信息"
            };
            return View(studentDetailsViewModel);
        }

        //通过Get请求访问视图页面
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //重定向实体,然后在创建视图页面中创建实体,通过按钮提交,存储到存储体中
        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            //如果验证失败,则返回相同的视图,以便用户可以提供所需的数据并重新提交表单
            if (ModelState.IsValid)
            {
                String uniqueFileName = null;

                if (model.Photo != null)
                {
                    uniqueFileName = ProcessUploadedFile(model);
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    ClassName = model.ClassName,
                    PhotoPath = uniqueFileName
                };
                _studentRepository.Add(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }

            return View();
        }

        //1.视图
        //视图模型
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Student student = _studentRepository.GetStudent(id);

            if(student != null)
            {
                StudentEditViewModel studentEditViewModel = new StudentEditViewModel
                {
                    Id = student.Id,
                    Name = student.Name,
                    Email = student.Email,
                    ClassName = student.ClassName,
                    ExistingPhotoPath = student.PhotoPath
                };

                return View(studentEditViewModel);
            }
            throw new Exception("查询不到这个学生信息");
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            //检查提供的数据是否有效
            if (ModelState.IsValid)
            {
                Student student = _studentRepository.GetStudent(model.Id);
                student.Email = model.Email;
                student.ClassName = model.ClassName;
                string filepath = null;

                if (model.ExistingPhotoPath != null)
                {
                    filepath = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                    
                }
                
                if(ProcessUploadedFile(model) != null)
                {                              
                    student.PhotoPath = ProcessUploadedFile(model);
                    if (filepath != null)
                    {
                        System.IO.File.Delete(filepath);
                    }
                }
                Student updateStudent = _studentRepository.Update(student);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        /// <summary>
        /// 将照片保存到指定的路径中,并返回唯一的文件名
        /// </summary>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentCreateViewModel model)
        {
          
            string uniqueFileName = null;

            if(model.Photo != null)
            {
                //必须将图像上传到wwwroot中的images文件夹
                //而要获取wwwroot文件夹的路径,我们需要注入 ASP.NET Core提供的HostingEnvironment服务
                //通过HostingEnvironment服务去获取wwwroot文件夹的路径
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                //为了确保文件名是唯一的,我们在文件名后附加一个新的GUID值和一个下划线

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //因为使用了非托管资源,所以需要手动进行释放
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    //使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images文件夹
                    model.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }//
    }
}
