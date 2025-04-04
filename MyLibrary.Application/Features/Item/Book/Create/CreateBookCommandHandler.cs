using MediatR;
using MyLibrary.Application.Features.Item.Abstraction.Create;
using MyLibrary.Application.FUTURE_API.Item.Book.Create;
using MyLibrary.Domain.Abstraction;
using MyLibrary.Domain.Item.Abstraction.Repository;
using MyLibrary.Domain.User;

namespace MyLibrary.Application.Features.Item.Book.Create;

public class CreateBookCommandHandler(ISender sender, IItemRepository<Domain.Item.Book.Book> itemRepository, IUnitOfWork unitOfWork)
    : CreateItemCommandHandler<Domain.Item.Book.Book, CreateBookCommand, CreateBookResponse>(sender, itemRepository, unitOfWork)
{
    protected override Domain.Item.Book.Book CreateItem(CreateBookCommand request, LibraryUser owner) =>
        Domain.Item.Book.Book.Create(request.Name, request.Author, request.Year, request.Isbn, request.Description, owner);

    protected override CreateBookResponse CreateResponse(Domain.Item.Book.Book item) =>
        new(item.Id, item.Name, item.Description, item.Owner.Id);
}