import * as React from "react";
import { ResponseMessage } from "../Response/Response";
import mermaid from "mermaid";

mermaid.initialize({
  startOnLoad: true,
});

export default function ResponseGraph({ response }: { response: ResponseMessage }) {
    let data = ConvertToMermaidDiagram(response);

    React.useEffect(() => {
        mermaid.initialize({ sequence: { showSequenceNumbers: true } });
        mermaid.contentLoaded();
    }, []);

    return (
    <div className="mermaid">
        { data }         
    </div>
  );
}

function ConvertToMermaidDiagram(response: ResponseMessage) {
    let chatMessages = "";
    let pluginFunctionNames: Set<string> = new Set<string>();
    let pluginFunctions = "";

    response.chatHistory.forEach((message) => {
        if (message.role === "assistant" && message.functionName != "") {
            pluginFunctionNames.add(message.functionName);
        }
    });

    pluginFunctionNames.forEach((functionName) => {
        pluginFunctions += `    participant ${functionName}\n`;
    });

    response.chatHistory.forEach((message, index, chatHistory) => {
        if (message.role === "system") {
            chatMessages += `    WebApp->>OpenAI: ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}\n`;
        }

        if (message.role === "assistant") {
            if (message.functionName != "") {
                chatMessages += `    WebApp->>${message.functionName}: ${message.functionArguments.replace(/(?:\r\n|\r|\n)/g, '')}\n`;
            }
            else {
                chatMessages += `    WebApp->>OpenAI: ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}\n`;
            }
        }

        if (message.role === "user") {
            chatMessages += `    OpenAI->>WebApp: ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}\n`;
        }

        if (message.role === "tool") {
            chatMessages += `    ${chatHistory[index - 1].functionName}->>WebApp: ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}\n`;
        }
    }
    );
   
    let mermaidDiagram =
    `sequenceDiagram
         box Semantic Kernel
         participant WebApp
         participant OpenAI
         end
         box Plugins
         ${pluginFunctions}
         end
         ${chatMessages}

         `;

    return mermaidDiagram;
}