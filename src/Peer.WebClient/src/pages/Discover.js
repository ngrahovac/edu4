import React from 'react'
import SingleColumnLayout from '../layout/SingleColumnLayout'
import RefineButton from '../comps/discover/RefineButton';
import SelectedDiscoveryParameter from '../comps/discover/SearchFilter';
import SelectedDiscoveryParameters from '../comps/discover/SearchFilters';
import { useState, useEffect } from 'react';
import ProjectCard from '../comps/discover/ProjectCard';
import DiscoveryParametersSidebar from '../comps/discover/DiscoveryRefinementSidebar';
import { discover } from '../services/ProjectsService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { useAuth0 } from '@auth0/auth0-react';
import { me } from '../services/UsersService'
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';

const Discover = () => {
    const [projects, setProjects] = useState([]);

    const [ownHats, setOwnHats] = useState(undefined);
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [discoveryRefinementSidebarVisibility, setDiscoveryParametersSidebarVisibility] = useState(false);

    const [loading, setLoading] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [getAccessTokenSilently])

    useEffect(() => {
        const handleDiscoveryRefinementsChange = () => {
            (async () => {
                try {
                    setLoading(true);

                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await discover(keyword, sort, hat, token);
                    setLoading(false);

                    if (result.outcome === successResult) {
                        var projects = result.payload;
                        setProjects(projects);
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setLoading(false);
                }
            })();
        }

        handleDiscoveryRefinementsChange();
    }, [keyword, sort, hat, getAccessTokenSilently])

    function updateDiscoveryParameters(keyword, sort, hat) {
        setKeyword(keyword);
        setHat(hat);
        setSort(sort);
    }

    if (loading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={loading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        );
    }

    return (
        ownHats &&
        <SingleColumnLayout
            title="Discover projects"
            description="Something encouraging here">

            {/* refine button and selected discovery parameters */}
            <div className='flex flex-col mt-16'>
                <RefineButton onClick={() => setDiscoveryParametersSidebarVisibility(true)}></RefineButton>

                {/* selected discovery parameters */}
                <SelectedDiscoveryParameters>
                    {
                        keyword !== undefined &&
                        <SelectedDiscoveryParameter
                            value={`keyword: ${keyword}`}
                            onRemoved={() => setKeyword(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                    {
                        sort !== undefined &&
                        <SelectedDiscoveryParameter
                            value={`sort: ${sort === "asc" ? "oldest first" : "newest first"}`}
                            onRemoved={() => setSort(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                    {
                        hat !== undefined &&
                        <SelectedDiscoveryParameter
                            value={`looking for: a ${hat.type.toLowerCase()} like me`}
                            onRemoved={() => setHat(undefined)}>
                        </SelectedDiscoveryParameter>
                    }
                </SelectedDiscoveryParameters>
            </div>

            { /* discovery results */}
            <div className='mt-16'>
                {
                    projects.length > 0 &&
                    <div className='flex flex-col space-y-8'>
                        {
                            projects.map((p, index) => <div key={index}>
                                <ProjectCard project={p}></ProjectCard>
                            </div>)
                        }
                    </div>
                }

                {
                    projects.length <= 0 &&
                    <p>There are currently no projects satisfying the criteria.</p>
                }
            </div>

            { /* discovery parameters sidebar */}
            {
                discoveryRefinementSidebarVisibility &&
                <div className='fixed left-0 top-0'>
                    <DiscoveryParametersSidebar
                        keyword={keyword}
                        sort={sort}
                        hat={hat}
                        hats={ownHats}
                        onModalClosed={() => setDiscoveryParametersSidebarVisibility(false)}
                        onDiscoveryParametersChanged={updateDiscoveryParameters}>
                    </DiscoveryParametersSidebar>
                </div>
            }
        </SingleColumnLayout>
    )
}

export default Discover