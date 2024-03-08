import { getAsync } from "./ApiService";
import { failureResult, successResult } from "./RequestResult";

async function getNotifs(accessToken) {
    const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
    const notifsUri = `${apiRootUri}/notifications`;

    const response = await getAsync(notifsUri, accessToken);

    try {
        if (response.ok) {
            let notifs = await response.json();

            return {
                outcome: successResult,
                payload: notifs
            }
        } else {
            return {
                outcome: failureResult
            }
        }
    } catch (ex) {
        return {
            outcome: console.error()
        }
    }
}

export { getNotifs }