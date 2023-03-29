using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab6_NET.Data;
using Lab6_NET.Models;
using System.Xml.Linq;

namespace Lab6_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly StudentDbContext _context;

        public StudentsController(StudentDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        // GET: Students
        /// <summary>
        /// Get collection of Students.
        /// </summary>
        /// <returns>A colection of Students</returns>
        /// <response code="200">Returns a collection of Students</response>
        /// <response code="404">If the Student is null</response> 
        /// <response code="500">Internal error</response>  
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when we return list of Students successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] //this code confirms if the request exists
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request

        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        /// <summary>
        /// Get a Student.
        /// </summary>
        /// <param id="id"></param>
        /// <returns>A Student</returns>
        /// <response code="201">Returns a collection of Students</response>
        /// <response code="400">If the id is malformed</response>      
        /// <response code="404">If the Student is null</response>      
        /// <response code="500">Internal error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when we return list of Students successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // this code returns a bad request response, student is null
        [ProducesResponseType(StatusCodes.Status404NotFound)] //this code confirms if the request exists
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request

        public async Task<ActionResult<Student>> GetStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Upserts a Student.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Students
        ///     {
        ///        "FirstName": "FirstName",
        ///        "LastName": "LastName",
        ///        "Program": "Program"
        ///     }
        ///
        /// </remarks>
        /// <param id="id"></param>
        /// <returns>An upserted Car</returns>
        /// <response code="200">Returns the updated Student</response>
        /// <response code="201">Returns the newly created Student</response>
        /// <response code="400">If the Student or id is malformed</response>      
        /// <response code="500">Internal error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when we return list of Students successfully
        [ProducesResponseType(StatusCodes.Status201Created)] //returns when creation of a student is completed
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // this code returns a bad request response, student is null
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request

        //The method takes three parameters: a Guid called id that represents the ID of the Student object to be updated,
        //a Student object called student that is annotated with a [Bind] attribute to whitelist the properties that can be
        //updated, and the method returns an IActionResult object.
        public async Task<IActionResult> PutStudent(Guid id, [Bind("FirstName,LastName,Program")] Student student)
        {
            // ref: https://github.com/aarad-ac/WebApiCore/blob/main/WebAPI/Controllers/CarsController.cs
            //The method starts by creating a new Student object called students and initializing its FirstName,
            //LastName, and Program properties to the corresponding properties of the input student object.
            Student students = new Student
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Program = student.Program
            };

            //method checks if a Student object with the specified id exists in the database by calling the
            //StudentExists method.If the StudentExists method returns false, the code sets the ID property
            //of the students object to the specified id, adds the students object to the _context database
            //context, saves the changes to the database, and returns a CreatedAtAction result that specifies
            //the name of a method called GetStudent that returns the newly created students object
            if (!StudentExists(id))
            {
                students.ID = id;
                _context.Add(students);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetStudent), new { id = students.ID }, students);
            }

            //If the StudentExists method returns true, the code retrieves the existing Student object from
            //the database using the specified id and assigns its FirstName, LastName, and Program properties
            //to the corresponding properties of the input student object. Then, the code updates the
            //dbStudent object in the _context database context and saves the changes to the database.
            //Finally, the code returns an Ok result that contains the updated dbStudent object.
            Student dbStudent = await _context.Students.FindAsync(id);
            dbStudent.FirstName = student.FirstName;
            dbStudent.LastName = student.LastName;
            dbStudent.Program = student.Program;

            _context.Update(dbStudent);
            await _context.SaveChangesAsync();

            return Ok(dbStudent);

        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Creates a Student.
        /// </summary>

       // POST /Students
       ///     {
       ///        "FirstName": "FirstName",
       ///        "LastName": "LastName",
       ///        "Program": "Program"
       ///     }
       /// <returns>A newly created Student</returns>
       /// <response code="201">Returns the newly created Student</response>
       /// <response code="201">Returns the newly created Student</response>
       /// <response code="400">If the Student or id is malformed</response>        
       /// <response code="500">Internal error</response>
       [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when we return list of Students successfully
        [ProducesResponseType(StatusCodes.Status201Created)] //returns when creation of student is completed
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // this code returns a bad request response, student is null
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request

        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/Students/5
        /// <summary>
        /// Deletes a Student.
        /// </summary>
        /// <param id="id"></param>
        /// <response code="202">Student is deleted</response>
        /// <response code="400">If the id is malformed</response>      
        /// <response code="500">Internal error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // returned when a student is deleted succesfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // this code returns a bad request response, student is null
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // returned when there is an error in processing the request

        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(Guid id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
