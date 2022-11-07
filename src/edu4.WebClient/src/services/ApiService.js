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



export { postAsync };