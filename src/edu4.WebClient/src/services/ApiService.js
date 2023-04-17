async function getAsync(resourceUri, accessToken) {
    const params = {
        method: "GET",
        headers: {
            Authorization: `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        }
    };

    var response = await fetch(resourceUri, params);

    return response;
}

async function postAsync(resourceUri, payload, accessToken) {
    const params = {
        method: "POST",
        headers: {
            Authorization: `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    };

    var response = await fetch(resourceUri, params);
    return response;
}

async function putAsync(resourceUri, payload, accessToken) {
    const params = {
        method: "PUT",
        headers: {
            Authorization: `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    };

    var response = await fetch(resourceUri, params);
    return response;
}

async function deleteAsync(resourceUri, accessToken) {
    const params = {
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${accessToken}`,
            "Content-Type": "application/json"
        }
    };

    var response = await fetch(resourceUri, params);
    return response;
}



export { getAsync, postAsync, putAsync, deleteAsync };