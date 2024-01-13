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
    const [discoveredProjects, setDiscoveredProjects] = useState(undefined);

    const [ownHats, setOwnHats] = useState(undefined);
    const [keyword, setKeyword] = useState(undefined);
    const [sort, setSort] = useState(undefined);
    const [hat, setHat] = useState(undefined);
    const [discoveryRefinementSidebarVisibility, setDiscoveryParametersSidebarVisibility] = useState(false);

    const [pageLoading, setPageLoading] = useState(true);
    const [projectsLoading, setProjectsLoading] = useState(true);

    const { getAccessTokenSilently } = useAuth0();

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setPageLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setPageLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setPageLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [getAccessTokenSilently])

    useEffect(() => {
        const handleDiscoveryRefinementsChange = () => {
            (async () => {
                try {
                    setProjectsLoading(true);

                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await discover(keyword, sort, hat, token);
                    setProjectsLoading(false);

                    if (result.outcome === successResult) {
                        var projects = result.payload;
                        setDiscoveredProjects(projects);
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setProjectsLoading(false);
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

    if (pageLoading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={pageLoading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        );
    }

    const discoveryResults = projectsLoading ?
        <BeatLoader></BeatLoader> :
        !discoveredProjects.length ?
            <p>There are currently no projects satisfying the criteria.</p> :
            <div className='flex flex-col space-y-8'>
                {
                    discoveredProjects.map((p, index) => <div key={index}>
                        <ProjectCard project={p}></ProjectCard>
                    </div>)
                }
            </div>;

    return (
        ownHats &&
        <SingleColumnLayout
            title="Discover projects"
            description="Explore projects and find opportunities to contribute to">

            <div className='flex flex-col gap-y-8'>
                {/* refine button and selected discovery parameters 
            <div className='flex flex-col mt-16'>
                <RefineButton onClick={() => setDiscoveryParametersSidebarVisibility(true)}></RefineButton>

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
            */}

                {discoveryResults}
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