# Objectives of language processing step
**Input**: A document written by the user
**Objective**: Given a document, extract important keywords *from each phrase* of the document

## Methods:
- NLP (*Natural Language Processing*) techniques:
    - Keyword Extraction: Given a text, extract main keywords from it
    - NER (*Named Entity Recognition*): Given a text, extract names of places, events, people, and other entities.

# Tests

## Bypass workaround
**Method**: Instead of looking at the main words of each phrase, take every non-repeated word as a keyword.

### Testing results:
- Searching every word using current ligimgfetch isn't efficient. libimgfetch needs to become more efficient in order to handle several searches if we want this program's operation to be smooth
- In libimgfetch, perhaps more search options could be added, such that if one's quota runs out, others may fill in? This was one of the issues with the test: Google's CSE alone didn't work out for the entire article, making it not possible to use.
    - Looking into the possibility of adding RapidAPI's web search one
- There are several words that, once disconnected and splited from their context, don't convey the original meaning. This is why NLP **has** to be implemented. Otherwise we will end up with a bunch of images that don't mean anything.
- This tecnique, even if successful, would create a video with extremely rapidly changing images, which is not something very pleasant for the end user who is watching the video.
**In essence:** Splitting words one by one and searching them didn't work as expected and wouldn't result in a nice video anyway.

## Using a third-party pre-trained API
**Method**: Use an api from [APILayer.com](apilayer.com) in order to perform this function
**Drawbacks**: Extremely restricted quota, only 10 requests a day