using MyLibrary.Application.Features.Item.Abstraction.Create;
using MyLibrary.Application.FUTURE_API.Item.Book.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;

namespace MyLibrary.Application.Features.Item.Book.Create;

public class CreateBookCommandHandler(IItemRepository<Domain.Item.Book.Book> itemRepository, IUnitOfWork unitOfWork)
    : CreateItemCommandHandler<Domain.Item.Book.Book, CreateBookCommand, CreateBookResponse>(itemRepository, unitOfWork)
{
    protected override Domain.Item.Book.Book CreateItem(CreateBookCommand request, Guid owner) =>
        Domain.Item.Book.Book.Create(request.Name, request.Author, request.Year, request.Isbn, request.Description, owner);

    protected override CreateBookResponse CreateResponse(Domain.Item.Book.Book item) =>
        new(item.Id, item.Name, item.Description, item.Owner);
}