using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Rehber.Infrastructure.Dtos;
using Rehber.Infrastructure.Interfaces;
using Rehber.Infrastructure.Models;

namespace Rehber.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : Controller
{
    private readonly IPersonRepository _personRepository;
    private readonly IContactRepository _contactRepository;

    private readonly IMapper _mapper;
    // GET
    public PersonController(IPersonRepository personRepository, IMapper mapper, IContactRepository contactRepository)
    {
        _personRepository = personRepository;
        _mapper = mapper;
        _contactRepository = contactRepository;
    }

    
    [HttpPost("create")]
    public async Task<IActionResult> Create(PersonForCreateDto personForCreateDto)
    {
        if (string.IsNullOrEmpty(personForCreateDto.Name))
            return BadRequest("Person Name not exists");

        var personToCreate = _mapper.Map<Person>(personForCreateDto);
        personToCreate.CreatedAt=DateTime.Now;

        await _personRepository.Create(personToCreate);

        return Ok();
    }
    
    [HttpPost("remove")]
    public async Task<IActionResult> RemovePerson(Guid id)
    {
        if (id==null|| id==Guid.Empty)
            return BadRequest("Invalid Id");

        await _personRepository.Delete(id);

        return Ok();
    }
    
    [HttpPost("addcontactinfo")]
    public async Task<IActionResult> AddContactInfo(ContactForCreateDto contactForCreateDto)
    {
        if (contactForCreateDto.Type==null || string.IsNullOrEmpty(contactForCreateDto.Detail))
            return BadRequest("Invalid info");

        var contact = _mapper.Map<Contact>(contactForCreateDto);
        contact.PersonUUID = contactForCreateDto.PersonId;
        _contactRepository.Create(contact);

        return Ok();
    }
    
    [HttpPost("removecontactinfo")]
    public async Task<IActionResult> RemoveContactInfo(Guid id)
    {
        if (id==null|| id==Guid.Empty)
            return BadRequest("Invalid Id");

        await _contactRepository.Delete(id);

        return Ok();
    }
    
    [HttpPost("getall")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(_personRepository.GetAll());
    }
    
    [HttpPost("getpersoninfo")]
    public async Task<IActionResult> getPersonInfo(Guid id)
    {
        if (id==null|| id==Guid.Empty)
            return BadRequest("Invalid Id");
        
        var person = _personRepository.GetPersonAndContactsById(id);

        if (person ==null)
            return BadRequest("Invalid operation");
        
        return Ok();
        
    }
    
    public IActionResult Index()
    {
        return View();
    }
}