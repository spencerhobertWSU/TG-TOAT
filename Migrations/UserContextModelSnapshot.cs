﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TGTOAT.Data;

#nullable disable

namespace TGTOAT.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TGTOAT.Data.Address", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("AddressLineOne")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("AddressLineTwo")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("UserId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("TGTOAT.Data.Assignment", b =>
                {
                    b.Property<int>("AssignmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentId"));

                    b.Property<string>("AssignmentDescription")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("AssignmentName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("AssignmentPoints")
                        .HasColumnType("int");

                    b.Property<string>("AssignmentType")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DueDateAndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("InstructorCourseId")
                        .HasColumnType("int");

                    b.HasKey("AssignmentId");

                    b.HasIndex("CourseId");

                    b.HasIndex("InstructorCourseId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("TGTOAT.Data.Courses", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseId"));

                    b.Property<string>("Building")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Campus")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("CourseDescription")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("CourseNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("DaysOfTheWeek")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<TimeOnly?>("EndTime")
                        .IsRequired()
                        .HasColumnType("time");

                    b.Property<int>("NumberOfCredits")
                        .HasColumnType("int");

                    b.Property<int?>("RoomNumber")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Semester")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<TimeOnly?>("StartTime")
                        .IsRequired()
                        .HasColumnType("time");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("CourseId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("TGTOAT.Data.Departments", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentId"));

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");

                    b.HasData(
                        new
                        {
                            DepartmentId = 1,
                            DepartmentName = "Computer Science"
                        },
                        new
                        {
                            DepartmentId = 2,
                            DepartmentName = "Mathematics"
                        },
                        new
                        {
                            DepartmentId = 3,
                            DepartmentName = "Physics"
                        },
                        new
                        {
                            DepartmentId = 4,
                            DepartmentName = "Biology"
                        },
                        new
                        {
                            DepartmentId = 5,
                            DepartmentName = "Chemistry"
                        });
                });

            modelBuilder.Entity("TGTOAT.Data.InstructorCourseConnection", b =>
                {
                    b.Property<int>("InstructorCourseConnectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InstructorCourseConnectionId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("InstructorID")
                        .HasColumnType("int");

                    b.HasKey("InstructorCourseConnectionId");

                    b.HasIndex("CourseId");

                    b.HasIndex("InstructorID");

                    b.ToTable("InstructorCourseConnection");
                });

            modelBuilder.Entity("TGTOAT.Data.Notifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CoursesCourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CoursesCourseId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("TGTOAT.Data.StudentAssignments", b =>
                {
                    b.Property<int>("AssignmentGradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentGradeId"));

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<string>("FileSubmission")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Grade")
                        .HasColumnType("int");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TextSubmission")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("studentCourseConnectionStudentCourseId")
                        .HasColumnType("int");

                    b.HasKey("AssignmentGradeId");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("studentCourseConnectionStudentCourseId");

                    b.ToTable("StudentAssignment");
                });

            modelBuilder.Entity("TGTOAT.Data.StudentCourseConnection", b =>
                {
                    b.Property<int>("StudentCourseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentCourseId"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<decimal?>("Grade")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.HasKey("StudentCourseId");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentID");

                    b.ToTable("StudentCourseConnection");
                });

            modelBuilder.Entity("TGTOAT.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AmountDue")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CoursesCourseId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProfileImageBase64")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserRole")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CoursesCourseId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("TGTOAT.Data.Address", b =>
                {
                    b.HasOne("TGTOAT.Models.User", null)
                        .WithOne("Address")
                        .HasForeignKey("TGTOAT.Data.Address", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TGTOAT.Data.Assignment", b =>
                {
                    b.HasOne("TGTOAT.Data.Courses", "Course")
                        .WithMany("Assignments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TGTOAT.Data.InstructorCourseConnection", "InstructorCourse")
                        .WithMany()
                        .HasForeignKey("InstructorCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("InstructorCourse");
                });

            modelBuilder.Entity("TGTOAT.Data.Courses", b =>
                {
                    b.HasOne("TGTOAT.Data.Departments", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");
                });

            modelBuilder.Entity("TGTOAT.Data.InstructorCourseConnection", b =>
                {
                    b.HasOne("TGTOAT.Data.Courses", "Course")
                        .WithMany("instructorCourseConnections")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TGTOAT.Models.User", "Instructor")
                        .WithMany()
                        .HasForeignKey("InstructorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("TGTOAT.Data.Notifications", b =>
                {
                    b.HasOne("TGTOAT.Data.Courses", null)
                        .WithMany("Notifications")
                        .HasForeignKey("CoursesCourseId");
                });

            modelBuilder.Entity("TGTOAT.Data.StudentAssignments", b =>
                {
                    b.HasOne("TGTOAT.Data.Assignment", "Assignments")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TGTOAT.Data.StudentCourseConnection", "studentCourseConnection")
                        .WithMany()
                        .HasForeignKey("studentCourseConnectionStudentCourseId");

                    b.Navigation("Assignments");

                    b.Navigation("studentCourseConnection");
                });

            modelBuilder.Entity("TGTOAT.Data.StudentCourseConnection", b =>
                {
                    b.HasOne("TGTOAT.Data.Courses", "Course")
                        .WithMany("StudentCourseConnection")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TGTOAT.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TGTOAT.Models.User", b =>
                {
                    b.HasOne("TGTOAT.Data.Courses", null)
                        .WithMany("Instructors")
                        .HasForeignKey("CoursesCourseId");
                });

            modelBuilder.Entity("TGTOAT.Data.Courses", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Instructors");

                    b.Navigation("Notifications");

                    b.Navigation("StudentCourseConnection");

                    b.Navigation("instructorCourseConnections");
                });

            modelBuilder.Entity("TGTOAT.Models.User", b =>
                {
                    b.Navigation("Address")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
