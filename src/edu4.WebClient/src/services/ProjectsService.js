import { getAsync, postAsync} from './ApiService'
import {
    successResult,
    failureResult,
    errorResult
} from './RequestResult'

async function publish(publishModel, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects`, publishModel, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project published successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

async function addPositions(projectId, positions, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects/${projectId}/positions`, positions, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Positions added successfully!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}


async function discover(keyword, sort, hatType, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/projects`;

        var searchRefinemets = [];

        if (keyword != undefined)
            searchRefinemets["keyword"] = keyword;
        
        if (sort != undefined)
            searchRefinemets["sort"] = sort;

        if (hatType != undefined) 
            searchRefinemets["hatType"] = hatType;

        if (Object.keys(searchRefinemets).length > 0) {
            requestUri += "?";

            for (let parameter in searchRefinemets) 
                if (searchRefinemets[parameter] != undefined)
                    requestUri += `${parameter}=${searchRefinemets[parameter]}&`    
            
            requestUri = requestUri.slice(0, -1);
        }            

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            var body = await response.json();

            return {
                outcome: successResult,
                message: "Signup successfully completed!",
                payload: body
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

export { publish, addPositions, discover }