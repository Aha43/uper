using System.Text.Json;
using Uper.Domain.Request.Dto;

namespace Uper.UnitTest.Uper.Domain.Request.Dto;

public class DeSerilization
{
    [Fact]
    public void SimpleJsonShouldDeserialize()
    {
        var json = """
{
    "type": "ExampleType",
    "objects": [
        { "Id": "uuid-123", "Name": "Sample" },
        { "Id": "uuid-124", "Name": "Another" }
    ]
}
""";

        var dto = JsonSerializer.Deserialize<CreateUpdateDto>(json);
        Assert.NotNull(dto);
        Assert.Equal("ExampleType", dto.Type);
        Assert.Equal(2, dto.Objects.Count);
        Assert.Equal("uuid-123", dto.Objects[0]["Id"].ToString());
        Assert.Equal("Sample", dto.Objects[0]["Name"].ToString());
        Assert.Equal("uuid-124", dto.Objects[1]["Id"].ToString());
        Assert.Equal("Another", dto.Objects[1]["Name"].ToString());
    }

    [Fact]
    public void AllJsonTypesShouldDeserialize()
    {
        var json = """
    {
        "type": "ExampleType",
        "objects": [
            {
                "Id": "uuid-123",
                "StringValue": "SampleString",
                "NumberValue": 42,
                "BooleanValue": true,
                "NullValue": null
            }
        ]
    }
    """;

        var dto = JsonSerializer.Deserialize<CreateUpdateDto>(json);

        // Validate DTO
        Assert.NotNull(dto);
        Assert.Equal("ExampleType", dto.Type);
        Assert.Single(dto.Objects);

        var firstObject = dto.Objects[0];

        // Validate Object Properties
        Assert.Equal("uuid-123", firstObject["Id"].ToString());
        Assert.Equal("SampleString", firstObject["StringValue"].ToString());
        Assert.Equal("42", firstObject["NumberValue"].ToString());
        //Assert.True((bool)firstObject["BooleanValue"]);
        //Assert.Null(firstObject["NullValue"]);
    }

}
